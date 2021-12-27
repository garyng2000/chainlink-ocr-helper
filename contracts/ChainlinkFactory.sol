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
    string memory _description,
    address _newOwner
  ) external;

  function transferOwnership(address _to) external;
}

interface KeeperRegistryInterface {
  function initialize(
    address link,
    address linkEthFeed,
    address fastGasFeed,
    uint32 paymentPremiumPPB,
    uint24 blockCountPerTurn,
    uint32 checkGasLimit,
    uint24 stalenessSeconds,
    uint16 gasCeilingMultiplier,
    uint256 fallbackGasPrice,
    uint256 fallbackLinkPrice,
    address newOwner
  ) external; 
  function transferOwnership(address _to) external;
}

interface VRFCoordinatorInterface {
  function initialize(
    address _link, 
    address _blockHashStore,
    address newOwner
  ) external;
  function transferOwnership(address _to) external;
}

interface OperatorInterface {
  function initialize(address link, address owner) external;
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

contract ChainlinkFactory is Owned {

    address public ocrImplementation;
    address public keeperImplementation;
    address public operatorImplementation;
    address public vrfImplementation;
    uint256 public ocrFee;
    uint256 public keeperFee;
    uint256 public operatorFee;
    uint256 public vrfFee;

    event OCRAggregtorCreated(
        address indexed aggregator,
        address indexed owner,
        address indexed sender
    );
    event KeeperRegistryCreated(
        address indexed keeperRegistry,
        address indexed owner,
        address indexed sender
    );
    event VRFCoordinatorCreated(
        address indexed vrfCoordinator,
        address indexed owner,
        address indexed sender
    );
    event OperatorCreated(
        address indexed operator,
        address indexed owner,
        address indexed sender
    );
    event NewContractCreated(
        address indexed implementation,
        address indexed newAddress
    );

    constructor() {
        initialize();
    }
    
    function initialize() public initializer {
        __Owned_init();
    }

    function setImpl(address[] calldata impl, uint256[] calldata fees) public onlyOwner {
        setOCRImpl(impl[0], fees[0]);
        setKeeperImpl(impl[1], fees[1]);
        setOperatorImpl(impl[2], fees[2]);
        setVRFImpl(impl[3], fees[3]);
    }

    function setOCRImpl(address impl, uint256 fee) public onlyOwner {
        ocrImplementation = impl;
        ocrFee = fee;
    }
    function setKeeperImpl(address impl, uint256 fee) public onlyOwner {
        keeperImplementation = impl;
        keeperFee = fee;
    }
    function setOperatorImpl(address impl, uint256 fee) public onlyOwner {
        operatorImplementation = impl;
        operatorFee = fee;
    }
    function setVRFImpl(address impl, uint256 fee) public onlyOwner {
        vrfImplementation = impl;
        vrfFee = fee;
    }

    function withdraw(uint256 amount) public onlyOwner {
        msg.sender.transfer(amount);
    }

    function cloneAndInit(address implementation, bool copy, bytes calldata init) public returns(address) {
        address newContract = copy ? Clones.copy(implementation) : Clones.clone(implementation);
        if (init.length > 0) {
            // solhint-disable-next-line avoid-low-level-calls
            (bool result, ) = newContract.call(init);
            require(result,"failed to call init");
        }
        emit NewContractCreated(implementation, newContract);
        return newContract;
    }

    function cloneKeeperRegistry(    
        address link,
        address linkEthFeed,
        address fastGasFeed,
        uint32 paymentPremiumPPB,
        uint24 blockCountPerTurn,
        uint32 checkGasLimit,
        uint24 stalenessSeconds,
        uint16 gasCeilingMultiplier,
        uint256 fallbackGasPrice,
        uint256 fallbackLinkPrice,
        bool copy
    ) external payable returns (address) {
        require(msg.value >= keeperFee,"need fee to clone");
        KeeperRegistryInterface cloned = KeeperRegistryInterface(copy ? Clones.copy(keeperImplementation) : Clones.clone(keeperImplementation));
        cloned.initialize(
            link, 
            linkEthFeed, 
            fastGasFeed, 
            paymentPremiumPPB, 
            blockCountPerTurn, 
            checkGasLimit, 
            stalenessSeconds, 
            gasCeilingMultiplier, 
            fallbackGasPrice, 
            fallbackLinkPrice, 
            msg.sender);
        emit KeeperRegistryCreated(
            address(cloned),
            msg.sender,
            msg.sender
        );
        return address(cloned);
    }
    function cloneVRFCoordinator(
        address _link, 
        address _blockHashStore,
        bool copy
    ) external payable returns (address) {
        require(msg.value >= vrfFee,"need fee to clone");
        VRFCoordinatorInterface cloned = VRFCoordinatorInterface(copy ? Clones.copy(vrfImplementation) : Clones.clone(vrfImplementation));
        cloned.initialize(_link, _blockHashStore, msg.sender);
        emit VRFCoordinatorCreated(
            address(cloned),
            msg.sender,
            msg.sender
        );
        return address(cloned);
    }
    function cloneOperator(
        address link,
        bool copy
    ) external payable returns (address) {
        require(msg.value >= operatorFee,"need fee to clone");
        OperatorInterface cloned = OperatorInterface(copy ? Clones.copy(operatorImplementation) : Clones.clone(operatorImplementation));
        cloned.initialize(link, msg.sender);
        emit OperatorCreated(
            address(cloned),
            msg.sender,
            msg.sender
        );
        return address(cloned);
    }

    function cloneOCRAggregtor(
        int192 _minValue,
        int192 _maxValue,
        uint8 _decimals,
        LinkTokenInterface _link,
        AccessControllerInterface _billingAccessController,
        AccessControllerInterface _requesterAccessController,
        string memory _description,
        bool copy
    ) external payable returns (address) {
        require(msg.value >= ocrFee,"need fee to clone");
        NewAggregatorParams memory params = NewAggregatorParams(_minValue, _maxValue, _decimals, _link, _billingAccessController, _requesterAccessController, _description);
        return _cloneOCRAggregtor(params, copy);
    }

    function _cloneOCRAggregtor(NewAggregatorParams memory params, bool copy) internal returns(address) {
        AccessControlledOffchainAggregatorInterface cloned = AccessControlledOffchainAggregatorInterface(
                copy ? Clones.copy(ocrImplementation) : Clones.clone(ocrImplementation)
                );
        _setupAggregator(cloned, params);
        return address(cloned);
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
            params.description,           // description
            msg.sender
            );

        //aggregator.transferOwnership(msg.sender);
        emit OCRAggregtorCreated(
            address(aggregator),
            msg.sender,
            msg.sender
        );
    }
}
