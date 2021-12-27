// SPDX-License-Identifier: MIT
pragma solidity >=0.7.0;
pragma abicoder v2;

import "./Clones.sol";
import "./Initializable.sol";
import "./Owned.sol";
//import "./libocr/contracts/AccessControlledOffchainAggregator.sol";
import "./libocr/contracts/AccessControllerInterface.sol";
import "./libocr/contracts/LinkTokenInterface.sol";

interface AccessControlledOffchainAggregatorInterface {
    function initialize(    
    uint32 _maximumGasPrice,
    uint32 _reasonableGasPrice,
    uint32 _microLinkPerEth,
    uint32 _linkGweiPerObservation,
    uint32 _linkGweiPerTransmission,
    LinkTokenInterface _link,
    int192 _minAnswer,
    int192 _maxAnswer,
    AccessControllerInterface _billingAccessController,
    AccessControllerInterface _requesterAccessController,
    uint8 _decimals,
    string memory _description
  ) external;

  function transferOwnership(address _to) external;
}
struct NewAggregatorParams {
    int192 minValue;
    int192 maxValue;
    uint8 decimals;
    LinkTokenInterface linkToken;
    AccessControllerInterface billingAccessController;
    AccessControllerInterface requesterAccessController;
    string description;
}

struct SharedSecretEncryptions {
    bytes32 diffieHellmanPoint;
    bytes32 sharedSecretHash;
    bytes16[] encryptions;
}

struct SetConfigEncodedComponents {
    uint64 deltaProgress;
    uint64 deltaResend;
    uint64 deltaRound;
    uint64 deltaGrace;
    uint64 deltaC;
    uint64 alphaPPB;
    uint64 deltaStage;
    uint8 rMax;
    uint8[] s;
    bytes32[] offchainPublicKeys;
    string peerIDs;
    SharedSecretEncryptions sharedSecretEncryptions;
}

contract OCRFactory is Owned {

    address public ocrImplementation;
    address public getChainlinkToken;
    address public writeAccessController;
    uint256 public fee;

    mapping(address => bool) private s_created;

    event OCRAggregtorCreated(
        address indexed aggregator,
        address indexed owner,
        address indexed sender
    );

    constructor(address linkAddress, address implementation, address writeAccessImpl) {
        initialize(linkAddress, implementation, writeAccessImpl);
    }
    
    function initialize(address linkAddress, address implementation, address writeAccessImpl) public initializer {
        __Owned_init();
        getChainlinkToken = linkAddress;
        ocrImplementation = implementation;
        writeAccessController = writeAccessImpl;
    }

    function setFee(uint256 amount) public onlyOwner {
        fee = amount;
    }

    function withdraw(uint256 amount) public onlyOwner {
        msg.sender.transfer(amount);
    }

    function cloneOCRAggregtor(
        int192 _minValue,
        int192 _maxValue,
        uint8 _decimals,
        LinkTokenInterface _link,
        AccessControllerInterface _billingAccessController,
        AccessControllerInterface _requesterAccessController,
        string memory _description
    ) external payable returns (address) {
        require(msg.value >= fee,"need fee to clone");
        NewAggregatorParams memory params = NewAggregatorParams(_minValue, _maxValue, _decimals, _link, _billingAccessController, _requesterAccessController, _description);
        return _cloneOCRAggregtor(params);
    }

    function _cloneOCRAggregtor(NewAggregatorParams memory params) internal returns(address) {
        AccessControlledOffchainAggregatorInterface cloned = AccessControlledOffchainAggregatorInterface(Clones.clone(ocrImplementation));
        _setupAggregator(cloned, params);
        return address(cloned);
    }

    /**
    * @notice creates a new Aggregator contract with the msg.sender as owner
    */
    function deployNewAggregator(
        int192 _minValue,
        int192 _maxValue,
        uint8 _decimals,
        LinkTokenInterface _link,
        AccessControllerInterface _billingAccessController,
        AccessControllerInterface _requesterAccessController,
        string memory _description
    )
        external payable
        returns (
        address
        )
    {
        require(msg.value >= fee,"need fee to clone");
        NewAggregatorParams memory params = NewAggregatorParams(_minValue, _maxValue, _decimals, _link, _billingAccessController, _requesterAccessController, _description);
        address newAggregator = _deployNewAggregator(params);

        return address(newAggregator);
    }

    function _makeConfigEncodedComponents( 
        uint64[7] memory config,
        uint8 rMax,
        uint8[] memory s,
        bytes32[] memory offchainPublicKeys,
        string memory peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] memory encryptions
        ) private pure returns(SetConfigEncodedComponents memory encodedComponenents) {            
        SharedSecretEncryptions memory sse = SharedSecretEncryptions(diffieHellmanPoint,sharedSecretHash,encryptions);
        encodedComponenents = SetConfigEncodedComponents({
            deltaProgress : uint64(config[0]),
            deltaResend : uint64(config[1]),
            deltaRound : uint64(config[2]),
            deltaGrace : uint64(config[3]),
            deltaC : uint64(config[4]),
            alphaPPB : uint64(config[5]),
            deltaStage : uint64(config[6]),
            rMax : rMax,
            s : s,
            offchainPublicKeys: offchainPublicKeys,
            peerIDs: peerIDs,
            sharedSecretEncryptions: sse
        });
    }
    function packDeltaComponent(
        uint64 deltaProgressNS,
        uint64 deltaResendNS,
        uint64 deltaRoundNS,
        uint64 deltaGraceNS,
        uint64 deltaC,
        uint64 alphaPPB,
        uint64 deltaStage
    ) public pure returns(uint64[7] memory packed) {
        /* chainlink used default: 35, 17, 30, 12(first 4, mainly retry related for each round, i.e. transmit)
           deltaC(control transmit frequency) and deltaStage controls frequency between submission 
           alphaPPB(%) controls 'rate change submit', in 1e9 so 1% is 1e7
         */
        require(deltaGraceNS < deltaRoundNS, "deltaGrace < deltaRound");
        require(deltaRoundNS < deltaProgressNS, "deltaRound < deltaProgress");
        packed[0] = deltaProgressNS;
        packed[1] = deltaResendNS;
        packed[2] = deltaRoundNS;
        packed[3] = deltaGraceNS;
        packed[4] = deltaC;
        packed[5] = alphaPPB;
        packed[6] = deltaStage;
    }

    function getDeltaParams(
        uint8 networkType,
        uint64 alphaPPB
    ) public pure returns(uint64[7] memory packed) {
        /* chainlink used default: 35, 17, 30, 12(first 4, mainly retry related for each round, i.e. transmit)
           deltaC(control transmit frequency) and deltaStage controls frequency between submission 
           alphaPPB(%) controls 'rate change submit', in 1e9 so 1% is 1e7
         */
        uint64 secondInNS = 1000000000;
        // these are the limits hardcoded inside libocr by chainId, value must be <= given with further restriction for grace/round/progress(see pack above)
        if (networkType == 1) {
            // moderate most POA
            // return packDeltaComponent(23 * secondInNS, 10 * secondInNS, 20 * secondInNS, 15 * secondInNS, 1 * 60 * secondInNS, alphaPPB, 5 * secondInNS);
            return packDeltaComponent(35 * secondInNS, 17 * secondInNS, 30 * secondInNS, 12 * secondInNS, 10 * 60 * secondInNS, alphaPPB, 10 * secondInNS);
        }
        else if (networkType == 2) {
            // fast say BSC
            // return packDeltaComponent(8 * secondInNS, 5 * secondInNS, 5 * secondInNS, 3 * secondInNS, 10 * secondInNS, alphaPPB, 5 * secondInNS);
            return packDeltaComponent(35 * secondInNS, 17 * secondInNS, 30 * secondInNS, 12 * secondInNS, 1 * 60 * secondInNS, alphaPPB, 10 * secondInNS);
        }
        else if (networkType == 3) {
            // public testnet(most, this is very fast)
            // return packDeltaComponent(2 * secondInNS, 2 * secondInNS, 1 * secondInNS, (1 * secondInNS)/2, 10 * secondInNS, alphaPPB, 5 * secondInNS);
            return packDeltaComponent(35 * secondInNS, 17 * secondInNS, 30 * secondInNS, 12 * secondInNS, 1 * 60 * secondInNS, alphaPPB, 10 * secondInNS);
        }
        else if (networkType == 4) {
            // super slow(no time based, 50 years between rounds)
            // return packDeltaComponent(23 * secondInNS, 10 * secondInNS, 20 * secondInNS, 15 * secondInNS, 10 * 60 * secondInNS, alphaPPB, 10 * secondInNS);
            return packDeltaComponent(35 * secondInNS, 17 * secondInNS, 30 * secondInNS, 12 * secondInNS, 60 * 60 * 24 * (3650 * 5 - 1) * secondInNS, alphaPPB, 30 * secondInNS);
        }
        else {
           // default slow, mainnet and private unknown
            // return packDeltaComponent(23 * secondInNS, 10 * secondInNS, 20 * secondInNS, 15 * secondInNS, 10 * 60 * secondInNS, alphaPPB, 10 * secondInNS);
            return packDeltaComponent(35 * secondInNS, 17 * secondInNS, 30 * secondInNS, 12 * secondInNS, 60 * 60 * secondInNS, alphaPPB, 30 * secondInNS);
        }
    }

    function makeSlowSetConfigEncodedComponents(
        uint64 alphaPPB,
        uint8 rMax,
        uint8[] calldata s,
        bytes32[] calldata offchainPublicKeys,
        string calldata peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] calldata encryptions
        ) public pure returns(bytes memory encodedComponenents) {  
        return makeSetConfigEncodedComponents(getDeltaParams(0, alphaPPB), rMax, s, offchainPublicKeys, peerIDs, diffieHellmanPoint, sharedSecretHash, encryptions);  
    }
    function makeModerateSetConfigEncodedComponents(
        uint64 alphaPPB,
        uint8 rMax,
        uint8[] calldata s,
        bytes32[] calldata offchainPublicKeys,
        string calldata peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] calldata encryptions
        ) public pure returns(bytes memory encodedComponenents) {  
        return makeSetConfigEncodedComponents(getDeltaParams(1, alphaPPB), rMax, s, offchainPublicKeys, peerIDs, diffieHellmanPoint, sharedSecretHash, encryptions);  
    }
    function makeFastSetConfigEncodedComponents(
        uint64 alphaPPB,
        uint8 rMax,
        uint8[] calldata s,
        bytes32[] calldata offchainPublicKeys,
        string calldata peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] calldata encryptions
        ) public pure returns(bytes memory encodedComponenents) {  
        return makeSetConfigEncodedComponents(getDeltaParams(2, alphaPPB), rMax, s, offchainPublicKeys, peerIDs, diffieHellmanPoint, sharedSecretHash, encryptions);  
    }
    function makeTestnetSetConfigEncodedComponents(
        uint64 alphaPPB,
        uint8 rMax,
        uint8[] calldata s,
        bytes32[] calldata offchainPublicKeys,
        string calldata peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] calldata encryptions
        ) public pure returns(bytes memory encodedComponenents) {  
        return makeSetConfigEncodedComponents(getDeltaParams(3, alphaPPB), rMax, s, offchainPublicKeys, peerIDs, diffieHellmanPoint, sharedSecretHash, encryptions);  
    }

    function makeSetConfigEncodedComponents( 
        // uint64 deltaProgress,
        // uint64 deltaResend,
        // uint64 deltaRound,
        // uint64 deltaGrace,
        // uint64 deltaC,
        // uint64 alphaPPB,
        // uint64 deltaStage,
        uint64[7] memory config,
        uint8 rMax,
        uint8[] memory s,
        bytes32[] memory offchainPublicKeys,
        string memory peerIDs,
        bytes32 diffieHellmanPoint,
        bytes32 sharedSecretHash,
        bytes16[] memory encryptions
        ) public pure returns(bytes memory encodedComponenents) {   
        encodedComponenents = abi.encode(
            SetConfigEncodedComponents({
            deltaProgress : uint64(config[0]),
            deltaResend : uint64(config[1]),
            deltaRound : uint64(config[2]),
            deltaGrace : uint64(config[3]),
            deltaC : uint64(config[4]),
            alphaPPB : uint64(config[5]),
            deltaStage : uint64(config[6]),
            rMax : rMax,
            s : s,
            offchainPublicKeys: offchainPublicKeys,
            peerIDs: peerIDs,
            sharedSecretEncryptions: SharedSecretEncryptions(diffieHellmanPoint,sharedSecretHash,encryptions)
        }));
    }

    function setConfigEncodedComponents(
        SetConfigEncodedComponents calldata components
    ) public pure returns(bytes memory) {
        return abi.encode(components);
    }

    function _deployNewAggregator(NewAggregatorParams memory params) internal returns(address) {
        address aggregator = _clone(ocrImplementation);
        _setupAggregator(AccessControlledOffchainAggregatorInterface(aggregator), params);
        return aggregator;
    }

    function _setupAggregator(AccessControlledOffchainAggregatorInterface aggregator, NewAggregatorParams memory params) internal {
        aggregator.initialize(
            /* billing params(5 below) can be revised later, here are the default */
            1000,             // _maximumGasPrice uint32(GWei?),
            20,              //_reasonableGasPrice uint32(GWei?),
            3.6e7,            // _microLinkPerEth, 3.6e7 microLINK, or 36 LINK
            1e8,              // _linkGweiPerObservation uint32,
            4e8,              // _linkGweiPerTransmission uint32,
            params.linkToken,          //_link Token Address,
            params.minValue,              // -2**191 
            params.maxValue,              // 2**191 - 1
            params.billingAccessController,       // _billingAccessController,
            params.requesterAccessController,       // _requesterAccessController,
            params.decimals,                   // _decimals,
            params.description           // description
            );

        s_created[address(aggregator)] = true;
        aggregator.transferOwnership(msg.sender);
        emit OCRAggregtorCreated(
            address(aggregator),
            msg.sender,
            msg.sender
        );
    }

    function _clone(address a) internal returns(address){

    /*
    https://gist.github.com/holiman/069de8d056a531575d2b786df3345665
    this is dup, not proxy

    Assembly of the code that we want to use as init-code in the new contract, 
    along with stack values:
	                # bottom [ STACK ] top
	 PUSH1 00       # [ 0 ]
	 DUP1           # [ 0, 0 ]
	 PUSH20         
	 <address>      # [0,0, address] 
	 DUP1		# [0,0, address ,address]
	 EXTCODESIZE    # [0,0, address, size ]
	 DUP1           # [0,0, address, size, size]
	 SWAP4          # [ size, 0, address, size, 0]
	 DUP1           # [ size, 0, address ,size, 0,0]
	 SWAP2          # [ size, 0, address, 0, 0, size]
	 SWAP3          # [ size, 0, size, 0, 0, address]
	 EXTCODECOPY    # [ size, 0]
	 RETURN 
    
    The code above weighs in at 33 bytes, which is _just_ above fitting into a uint. 
    So a modified version is used, where the initial PUSH1 00 is replaced by `PC`. 
    This is one byte smaller, and also a bit cheaper Wbase instead of Wverylow. It only costs 2 gas.

	 PC             # [ 0 ]
	 DUP1           # [ 0, 0 ]
	 PUSH20         
	 <address>      # [0,0, address] 
	 DUP1		# [0,0, address ,address]
	 EXTCODESIZE    # [0,0, address, size ]
	 DUP1           # [0,0, address, size, size]
	 SWAP4          # [ size, 0, address, size, 0]
	 DUP1           # [ size, 0, address ,size, 0,0]
	 SWAP2          # [ size, 0, address, 0, 0, size]
	 SWAP3          # [ size, 0, size, 0, 0, address]
	 EXTCODECOPY    # [ size, 0]
	 RETURN 

	The opcodes are:
	58 80 73 <address> 80 3b 80 93 80 91 92 3c F3
	We get <address> in there by OR:ing the upshifted address into the 0-filled space. 
	  5880730000000000000000000000000000000000000000803b80938091923cF3 
	 +000000xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx000000000000000000
	 -----------------------------------------------------------------
	  588073xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx00000803b80938091923cF3

	This is simply stored at memory position 0, and create is invoked. 

	*/
        address retval;
        assembly{
            mstore(0x0, or (0x5880730000000000000000000000000000000000000000803b80938091923cF3 , mul(a,0x1000000000000000000)))
            retval := create(0, 0, 32)
            switch extcodesize(retval) case 0 { revert(0, 0) }
        }
        return retval;
    }  

    function _clone2(address a, uint256 salt) internal returns (address) {
        /* this is dup, not proxy */
        address retval;
        assembly {
        mstore(0x0, or(0x5880730000000000000000000000000000000000000000803b80938091923cF3, mul(a, 0x1000000000000000000)))
        retval := create2(0, 0, 0x20, salt)
        switch extcodesize(retval) case 0 { revert(0, 0) }
        }
        return retval;
    }  
}
