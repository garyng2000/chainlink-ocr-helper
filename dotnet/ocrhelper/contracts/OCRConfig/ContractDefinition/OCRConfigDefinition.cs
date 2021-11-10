using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Chainlink.OCRConfig.ContractDefinition
{


    public partial class OCRConfigDeployment : OCRConfigDeploymentBase
    {
        public OCRConfigDeployment() : base(BYTECODE) { }
        public OCRConfigDeployment(string byteCode) : base(byteCode) { }
    }

    public class OCRConfigDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b506113bb806100206000396000f3fe608060405234801561001057600080fd5b50600436106100885760003560e01c80639338b1b71161005b5780639338b1b7146100fc5780639a1203fa1461010f578063a8c2a6ba14610122578063c272966e1461013557600080fd5b806325ec575d1461008d5780632f0bf56a146100b657806347fa6ebe146100d65780636213215b146100e9575b600080fd5b6100a061009b366004610a92565b610148565b6040516100ad9190610f02565b60405180910390f35b6100c96100c4366004610c6b565b610171565b6040516100ad9190610ec8565b6100a06100e4366004610b5a565b6102bd565b6100a06100f7366004610b5a565b6103d4565b6100a061010a3660046109aa565b6103e4565b6100a061011d366004610b5a565b610562565b6100c9610130366004610ad4565b610572565b6100a0610143366004610b5a565b61067f565b60608160405160200161015b9190610f15565b6040516020818303038152906040529050919050565b61017961068f565b633b9aca00600160ff851614156101d8576101d061019882601761131c565b6101a383600a61131c565b6101ae84601461131c565b6101b985600f61131c565b6101c486603c61131c565b8861013088600561131c565b9150506102b7565b8360ff166002141561021e576101d06101f282600861131c565b6101fd83600561131c565b61020884600561131c565b61021385600361131c565b6101c486600a61131c565b8360ff1660031415610270576101d061023882600261131c565b61024383600261131c565b61024e84600161131c565b600261025b86600161131c565b61026591906112e8565b6101c486600161131c565b6101d061027e82601761131c565b61028983600a61131c565b61029484601461131c565b61029f85600f61131c565b6102ab8661025861131c565b8861013088601461131c565b92915050565b60606103c36102cd60038f610171565b8d8d8d80806020026020016040519081016040528093929190818152602001838360200280828437600081840152601f19601f820116905080830192505050505050508c8c80806020026020016040519081016040528093929190818152602001838360200280828437600081840152601f19601f820116905080830192505050505050508b8b8080601f01602080910402602001604051908101604052809392919081815260200183838082843760009201919091525050604080516020808d0282810182019093528c82528f94508e935090918d918d9182918501908490808284376000920191909152506103e492505050565b9d9c50505050505050505050505050565b60606103c36102cd60018f610171565b60606040518061018001604052808a60006007811061040557610405611359565b60200201516001600160401b031681526020018a60016007811061042b5761042b611359565b60200201516001600160401b031681526020018a60026007811061045157610451611359565b60200201516001600160401b031681526020018a60036007811061047757610477611359565b60200201516001600160401b031681526020018a60046007811061049d5761049d611359565b60200201516001600160401b031681526020018a6005600781106104c3576104c3611359565b60200201516001600160401b031681526020018a6006600781106104e9576104e9611359565b60200201516001600160401b031681526020018960ff16815260200188815260200187815260200186815260200160405180606001604052808781526020018681526020018581525081525060405160200161054591906110ad565b604051602081830303815290604052905098975050505050505050565b60606103c36102cd60008f610171565b61057a61068f565b856001600160401b0316856001600160401b0316106105e05760405162461bcd60e51b815260206004820152601760248201527f64656c74614772616365203c2064656c7461526f756e6400000000000000000060448201526064015b60405180910390fd5b876001600160401b0316866001600160401b0316106106415760405162461bcd60e51b815260206004820152601a60248201527f64656c7461526f756e64203c2064656c746150726f677265737300000000000060448201526064016105d7565b6001600160401b03978816815295871660208701529386166040860152918516606085015284166080840152831660a083015290911660c082015290565b60606103c36102cd60028f610171565b6040518060e001604052806007906020820280368337509192915050565b60008083601f8401126106bf57600080fd5b5081356001600160401b038111156106d657600080fd5b6020830191508360208260051b85010111156106f157600080fd5b9250929050565b600082601f83011261070957600080fd5b8135602061071e61071983611219565b6111e9565b80838252828201915082860187848660051b890101111561073e57600080fd5b60005b8581101561076457610752826108b5565b84529284019290840190600101610741565b5090979650505050505050565b600082601f83011261078257600080fd5b8135602061079261071983611219565b80838252828201915082860187848660051b89010111156107b257600080fd5b60005b85811015610764578135845292840192908401906001016107b5565b600082601f8301126107e257600080fd5b60405160e081018181106001600160401b03821117156108045761080461136f565b604052808360e0810186101561081957600080fd5b60005b60078110156108435761082e82610982565b8352602092830192919091019060010161081c565b509195945050505050565b600082601f83011261085f57600080fd5b8135602061086f61071983611219565b80838252828201915082860187848660051b890101111561088f57600080fd5b60005b85811015610764576108a382610999565b84529284019290840190600101610892565b80356001600160801b0319811681146108cd57600080fd5b919050565b60008083601f8401126108e457600080fd5b5081356001600160401b038111156108fb57600080fd5b6020830191508360208285010111156106f157600080fd5b600082601f83011261092457600080fd5b81356001600160401b0381111561093d5761093d61136f565b610950601f8201601f19166020016111e9565b81815284602083860101111561096557600080fd5b816020850160208301376000918101602001919091529392505050565b80356001600160401b03811681146108cd57600080fd5b803560ff811681146108cd57600080fd5b6000806000806000806000806101c0898b0312156109c757600080fd5b6109d18a8a6107d1565b97506109df60e08a01610999565b96506101008901356001600160401b03808211156109fc57600080fd5b610a088c838d0161084e565b97506101208b0135915080821115610a1f57600080fd5b610a2b8c838d01610771565b96506101408b0135915080821115610a4257600080fd5b610a4e8c838d01610913565b95506101608b013594506101808b013593506101a08b0135915080821115610a7557600080fd5b50610a828b828c016106f8565b9150509295985092959890939650565b600060208284031215610aa457600080fd5b81356001600160401b03811115610aba57600080fd5b82016101808185031215610acd57600080fd5b9392505050565b600080600080600080600060e0888a031215610aef57600080fd5b610af888610982565b9650610b0660208901610982565b9550610b1460408901610982565b9450610b2260608901610982565b9350610b3060808901610982565b9250610b3e60a08901610982565b9150610b4c60c08901610982565b905092959891949750929550565b6000806000806000806000806000806000806101008d8f031215610b7d57600080fd5b610b868d610982565b9b50610b9460208e01610999565b9a506001600160401b0360408e01351115610bae57600080fd5b610bbe8e60408f01358f016106ad565b909a5098506001600160401b0360608e01351115610bdb57600080fd5b610beb8e60608f01358f016106ad565b90985096506001600160401b0360808e01351115610c0857600080fd5b610c188e60808f01358f016108d2565b909650945060a08d0135935060c08d013592506001600160401b0360e08e01351115610c4357600080fd5b610c538e60e08f01358f016106ad565b81935080925050509295989b509295989b509295989b565b60008060408385031215610c7e57600080fd5b610c8783610999565b9150610c9560208401610982565b90509250929050565b81835260006001600160fb1b03831115610cb757600080fd5b8260051b8083602087013760009401602001938452509192915050565b600081518084526020808501945080840160005b83811015610d0457815187529582019590820190600101610ce8565b509495945050505050565b8183526000602080850194508260005b85811015610d045760ff610d3283610999565b1687529582019590820190600101610d1f565b600081518084526020808501945080840160005b83811015610d0457815160ff1687529582019590820190600101610d59565b6000815180845260005b81811015610d9e57602081850181015186830182015201610d82565b81811115610db0576000602083870101525b50601f01601f19169290920160200192915050565b81835281816020850137506000828201602090810191909152601f909101601f19169091010190565b6000606083018235845260208084013581860152610e0f604085018561123c565b606060408801529283905291600090608087015b81831015610e52576001600160801b0319610e3d866108b5565b16815293830193600192909201918301610e23565b979650505050505050565b60006060830182518452602080840151818601526040840151606060408701528281518085526080880191508383019450600092505b80831015610ebd5784516001600160801b0319168252938301936001929092019190830190610e93565b509695505050505050565b60e08101818360005b6007811015610ef95781516001600160401b0316835260209283019290910190600101610ed1565b50505092915050565b602081526000610acd6020830184610d78565b60208152610f3660208201610f2984610982565b6001600160401b03169052565b6000610f4460208401610982565b6001600160401b038116604084015250610f6060408401610982565b6001600160401b038116606084015250610f7c60608401610982565b6001600160401b038116608084015250610f9860808401610982565b6001600160401b03811660a084015250610fb460a08401610982565b6001600160401b03811660c084015250610fd060c08401610982565b6001600160401b03811660e084015250610fec60e08401610999565b610100610ffd8185018360ff169052565b6110098186018661123c565b9250905061018061012081818701526110276101a087018585610d0f565b93506110358188018861123c565b93509050601f19610140818887030181890152611053868685610c9e565b9550611061818a018a611284565b955092505061016081888703018189015261107d868685610dc5565b955061108b818a018a6112c9565b9450508087860301838801525050506110a48282610dee565b95945050505050565b602081526110c76020820183516001600160401b03169052565b600060208301516110e360408401826001600160401b03169052565b5060408301516001600160401b03811660608401525060608301516001600160401b03811660808401525060808301516001600160401b03811660a08401525060a08301516001600160401b03811660c08401525060c08301516001600160401b03811660e08401525060e08301516101006111638185018360ff169052565b8085015191505061018061012081818601526111836101a0860184610d45565b9250808601519050601f196101408187860301818801526111a48584610cd4565b9450808801519250506101608187860301818801526111c38584610d78565b9088015187820390920184880152935090506111df8382610e5d565b9695505050505050565b604051601f8201601f191681016001600160401b03811182821017156112115761121161136f565b604052919050565b60006001600160401b038211156112325761123261136f565b5060051b60200190565b6000808335601e1984360301811261125357600080fd5b83016020810192503590506001600160401b0381111561127257600080fd5b8060051b36038313156106f157600080fd5b6000808335601e1984360301811261129b57600080fd5b83016020810192503590506001600160401b038111156112ba57600080fd5b8036038313156106f157600080fd5b60008235605e198336030181126112df57600080fd5b90910192915050565b60006001600160401b038084168061131057634e487b7160e01b600052601260045260246000fd5b92169190910492915050565b60006001600160401b038083168185168183048111821515161561135057634e487b7160e01b600052601160045260246000fd5b02949350505050565b634e487b7160e01b600052603260045260246000fd5b634e487b7160e01b600052604160045260246000fdfea264697066735822122009e8c2231b975f718d81574c76a103e4f0fed505e7b35d05f0fa501e72bebe4b64736f6c63430008060033";
        public OCRConfigDeploymentBase() : base(BYTECODE) { }
        public OCRConfigDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class GetDeltaParamsFunction : GetDeltaParamsFunctionBase { }

    [Function("getDeltaParams", "uint64[7]")]
    public class GetDeltaParamsFunctionBase : FunctionMessage
    {
        [Parameter("uint8", "networkType", 1)]
        public virtual byte NetworkType { get; set; }
        [Parameter("uint64", "alphaPPB", 2)]
        public virtual ulong AlphaPPB { get; set; }
    }

    public partial class MakeFastSetConfigEncodedComponentsFunction : MakeFastSetConfigEncodedComponentsFunctionBase { }

    [Function("makeFastSetConfigEncodedComponents", "bytes")]
    public class MakeFastSetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("uint64", "alphaPPB", 1)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint8", "rMax", 2)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 3)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 4)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 5)]
        public virtual string PeerIDs { get; set; }
        [Parameter("bytes32", "diffieHellmanPoint", 6)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 7)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 8)]
        public virtual List<byte[]> Encryptions { get; set; }
    }

    public partial class MakeModerateSetConfigEncodedComponentsFunction : MakeModerateSetConfigEncodedComponentsFunctionBase { }

    [Function("makeModerateSetConfigEncodedComponents", "bytes")]
    public class MakeModerateSetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("uint64", "alphaPPB", 1)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint8", "rMax", 2)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 3)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 4)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 5)]
        public virtual string PeerIDs { get; set; }
        [Parameter("bytes32", "diffieHellmanPoint", 6)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 7)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 8)]
        public virtual List<byte[]> Encryptions { get; set; }
    }

    public partial class MakeSetConfigEncodedComponentsFunction : MakeSetConfigEncodedComponentsFunctionBase { }

    [Function("makeSetConfigEncodedComponents", "bytes")]
    public class MakeSetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("uint64[7]", "config", 1)]
        public virtual List<ulong> Config { get; set; }
        [Parameter("uint8", "rMax", 2)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 3)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 4)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 5)]
        public virtual string PeerIDs { get; set; }
        [Parameter("bytes32", "diffieHellmanPoint", 6)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 7)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 8)]
        public virtual List<byte[]> Encryptions { get; set; }
    }

    public partial class MakeSlowSetConfigEncodedComponentsFunction : MakeSlowSetConfigEncodedComponentsFunctionBase { }

    [Function("makeSlowSetConfigEncodedComponents", "bytes")]
    public class MakeSlowSetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("uint64", "alphaPPB", 1)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint8", "rMax", 2)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 3)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 4)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 5)]
        public virtual string PeerIDs { get; set; }
        [Parameter("bytes32", "diffieHellmanPoint", 6)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 7)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 8)]
        public virtual List<byte[]> Encryptions { get; set; }
    }

    public partial class MakeTestnetSetConfigEncodedComponentsFunction : MakeTestnetSetConfigEncodedComponentsFunctionBase { }

    [Function("makeTestnetSetConfigEncodedComponents", "bytes")]
    public class MakeTestnetSetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("uint64", "alphaPPB", 1)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint8", "rMax", 2)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 3)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 4)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 5)]
        public virtual string PeerIDs { get; set; }
        [Parameter("bytes32", "diffieHellmanPoint", 6)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 7)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 8)]
        public virtual List<byte[]> Encryptions { get; set; }
    }

    public partial class PackDeltaComponentFunction : PackDeltaComponentFunctionBase { }

    [Function("packDeltaComponent", "uint64[7]")]
    public class PackDeltaComponentFunctionBase : FunctionMessage
    {
        [Parameter("uint64", "deltaProgressNS", 1)]
        public virtual ulong DeltaProgressNS { get; set; }
        [Parameter("uint64", "deltaResendNS", 2)]
        public virtual ulong DeltaResendNS { get; set; }
        [Parameter("uint64", "deltaRoundNS", 3)]
        public virtual ulong DeltaRoundNS { get; set; }
        [Parameter("uint64", "deltaGraceNS", 4)]
        public virtual ulong DeltaGraceNS { get; set; }
        [Parameter("uint64", "deltaC", 5)]
        public virtual ulong DeltaC { get; set; }
        [Parameter("uint64", "alphaPPB", 6)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint64", "deltaStage", 7)]
        public virtual ulong DeltaStage { get; set; }
    }

    public partial class SetConfigEncodedComponentsFunction : SetConfigEncodedComponentsFunctionBase { }

    [Function("setConfigEncodedComponents", "bytes")]
    public class SetConfigEncodedComponentsFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "components", 1)]
        public virtual SetConfigEncodedComponents Components { get; set; }
    }

    public partial class GetDeltaParamsOutputDTO : GetDeltaParamsOutputDTOBase { }

    [FunctionOutput]
    public class GetDeltaParamsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint64[7]", "packed", 1)]
        public virtual List<ulong> Packed { get; set; }
    }

    public partial class MakeFastSetConfigEncodedComponentsOutputDTO : MakeFastSetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class MakeFastSetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "encodedComponenents", 1)]
        public virtual byte[] EncodedComponenents { get; set; }
    }

    public partial class MakeModerateSetConfigEncodedComponentsOutputDTO : MakeModerateSetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class MakeModerateSetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "encodedComponenents", 1)]
        public virtual byte[] EncodedComponenents { get; set; }
    }

    public partial class MakeSetConfigEncodedComponentsOutputDTO : MakeSetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class MakeSetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "encodedComponenents", 1)]
        public virtual byte[] EncodedComponenents { get; set; }
    }

    public partial class MakeSlowSetConfigEncodedComponentsOutputDTO : MakeSlowSetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class MakeSlowSetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "encodedComponenents", 1)]
        public virtual byte[] EncodedComponenents { get; set; }
    }

    public partial class MakeTestnetSetConfigEncodedComponentsOutputDTO : MakeTestnetSetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class MakeTestnetSetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "encodedComponenents", 1)]
        public virtual byte[] EncodedComponenents { get; set; }
    }

    public partial class PackDeltaComponentOutputDTO : PackDeltaComponentOutputDTOBase { }

    [FunctionOutput]
    public class PackDeltaComponentOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint64[7]", "packed", 1)]
        public virtual List<ulong> Packed { get; set; }
    }

    public partial class SetConfigEncodedComponentsOutputDTO : SetConfigEncodedComponentsOutputDTOBase { }

    [FunctionOutput]
    public class SetConfigEncodedComponentsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }
}
