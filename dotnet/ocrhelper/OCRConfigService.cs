using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Chainlink.OCRConfig.ContractDefinition;

namespace Chainlink.OCRConfig
{
    public partial class OCRConfigService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, OCRConfigDeployment oCRConfigDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<OCRConfigDeployment>().SendRequestAndWaitForReceiptAsync(oCRConfigDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, OCRConfigDeployment oCRConfigDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<OCRConfigDeployment>().SendRequestAsync(oCRConfigDeployment);
        }

        public static async Task<OCRConfigService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, OCRConfigDeployment oCRConfigDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, oCRConfigDeployment, cancellationTokenSource);
            return new OCRConfigService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3 { get; set; }

        public ContractHandler ContractHandler { get; set; }

        public OCRConfigService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<List<ulong>> GetDeltaParamsQueryAsync(GetDeltaParamsFunction getDeltaParamsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetDeltaParamsFunction, List<ulong>>(getDeltaParamsFunction, blockParameter);
        }

        
        public Task<List<ulong>> GetDeltaParamsQueryAsync(byte networkType, ulong alphaPPB, BlockParameter blockParameter = null)
        {
            var getDeltaParamsFunction = new GetDeltaParamsFunction();
                getDeltaParamsFunction.NetworkType = networkType;
                getDeltaParamsFunction.AlphaPPB = alphaPPB;
            
            return ContractHandler.QueryAsync<GetDeltaParamsFunction, List<ulong>>(getDeltaParamsFunction, blockParameter);
        }

        public Task<byte[]> MakeFastSetConfigEncodedComponentsQueryAsync(MakeFastSetConfigEncodedComponentsFunction makeFastSetConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MakeFastSetConfigEncodedComponentsFunction, byte[]>(makeFastSetConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> MakeFastSetConfigEncodedComponentsQueryAsync(ulong alphaPPB, byte rMax, List<byte> s, List<byte[]> offchainPublicKeys, string peerIDs, byte[] diffieHellmanPoint, byte[] sharedSecretHash, List<byte[]> encryptions, BlockParameter blockParameter = null)
        {
            var makeFastSetConfigEncodedComponentsFunction = new MakeFastSetConfigEncodedComponentsFunction();
                makeFastSetConfigEncodedComponentsFunction.AlphaPPB = alphaPPB;
                makeFastSetConfigEncodedComponentsFunction.RMax = rMax;
                makeFastSetConfigEncodedComponentsFunction.S = s;
                makeFastSetConfigEncodedComponentsFunction.OffchainPublicKeys = offchainPublicKeys;
                makeFastSetConfigEncodedComponentsFunction.PeerIDs = peerIDs;
                makeFastSetConfigEncodedComponentsFunction.DiffieHellmanPoint = diffieHellmanPoint;
                makeFastSetConfigEncodedComponentsFunction.SharedSecretHash = sharedSecretHash;
                makeFastSetConfigEncodedComponentsFunction.Encryptions = encryptions;
            
            return ContractHandler.QueryAsync<MakeFastSetConfigEncodedComponentsFunction, byte[]>(makeFastSetConfigEncodedComponentsFunction, blockParameter);
        }

        public Task<byte[]> MakeModerateSetConfigEncodedComponentsQueryAsync(MakeModerateSetConfigEncodedComponentsFunction makeModerateSetConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MakeModerateSetConfigEncodedComponentsFunction, byte[]>(makeModerateSetConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> MakeModerateSetConfigEncodedComponentsQueryAsync(ulong alphaPPB, byte rMax, List<byte> s, List<byte[]> offchainPublicKeys, string peerIDs, byte[] diffieHellmanPoint, byte[] sharedSecretHash, List<byte[]> encryptions, BlockParameter blockParameter = null)
        {
            var makeModerateSetConfigEncodedComponentsFunction = new MakeModerateSetConfigEncodedComponentsFunction();
                makeModerateSetConfigEncodedComponentsFunction.AlphaPPB = alphaPPB;
                makeModerateSetConfigEncodedComponentsFunction.RMax = rMax;
                makeModerateSetConfigEncodedComponentsFunction.S = s;
                makeModerateSetConfigEncodedComponentsFunction.OffchainPublicKeys = offchainPublicKeys;
                makeModerateSetConfigEncodedComponentsFunction.PeerIDs = peerIDs;
                makeModerateSetConfigEncodedComponentsFunction.DiffieHellmanPoint = diffieHellmanPoint;
                makeModerateSetConfigEncodedComponentsFunction.SharedSecretHash = sharedSecretHash;
                makeModerateSetConfigEncodedComponentsFunction.Encryptions = encryptions;
            
            return ContractHandler.QueryAsync<MakeModerateSetConfigEncodedComponentsFunction, byte[]>(makeModerateSetConfigEncodedComponentsFunction, blockParameter);
        }

        public Task<byte[]> MakeSetConfigEncodedComponentsQueryAsync(MakeSetConfigEncodedComponentsFunction makeSetConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MakeSetConfigEncodedComponentsFunction, byte[]>(makeSetConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> MakeSetConfigEncodedComponentsQueryAsync(List<ulong> config, byte rMax, List<byte> s, List<byte[]> offchainPublicKeys, string peerIDs, byte[] diffieHellmanPoint, byte[] sharedSecretHash, List<byte[]> encryptions, BlockParameter blockParameter = null)
        {
            var makeSetConfigEncodedComponentsFunction = new MakeSetConfigEncodedComponentsFunction();
                makeSetConfigEncodedComponentsFunction.Config = config;
                makeSetConfigEncodedComponentsFunction.RMax = rMax;
                makeSetConfigEncodedComponentsFunction.S = s;
                makeSetConfigEncodedComponentsFunction.OffchainPublicKeys = offchainPublicKeys;
                makeSetConfigEncodedComponentsFunction.PeerIDs = peerIDs;
                makeSetConfigEncodedComponentsFunction.DiffieHellmanPoint = diffieHellmanPoint;
                makeSetConfigEncodedComponentsFunction.SharedSecretHash = sharedSecretHash;
                makeSetConfigEncodedComponentsFunction.Encryptions = encryptions;
            
            return ContractHandler.QueryAsync<MakeSetConfigEncodedComponentsFunction, byte[]>(makeSetConfigEncodedComponentsFunction, blockParameter);
        }

        public Task<byte[]> MakeSlowSetConfigEncodedComponentsQueryAsync(MakeSlowSetConfigEncodedComponentsFunction makeSlowSetConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MakeSlowSetConfigEncodedComponentsFunction, byte[]>(makeSlowSetConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> MakeSlowSetConfigEncodedComponentsQueryAsync(ulong alphaPPB, byte rMax, List<byte> s, List<byte[]> offchainPublicKeys, string peerIDs, byte[] diffieHellmanPoint, byte[] sharedSecretHash, List<byte[]> encryptions, BlockParameter blockParameter = null)
        {
            var makeSlowSetConfigEncodedComponentsFunction = new MakeSlowSetConfigEncodedComponentsFunction();
                makeSlowSetConfigEncodedComponentsFunction.AlphaPPB = alphaPPB;
                makeSlowSetConfigEncodedComponentsFunction.RMax = rMax;
                makeSlowSetConfigEncodedComponentsFunction.S = s;
                makeSlowSetConfigEncodedComponentsFunction.OffchainPublicKeys = offchainPublicKeys;
                makeSlowSetConfigEncodedComponentsFunction.PeerIDs = peerIDs;
                makeSlowSetConfigEncodedComponentsFunction.DiffieHellmanPoint = diffieHellmanPoint;
                makeSlowSetConfigEncodedComponentsFunction.SharedSecretHash = sharedSecretHash;
                makeSlowSetConfigEncodedComponentsFunction.Encryptions = encryptions;
            
            return ContractHandler.QueryAsync<MakeSlowSetConfigEncodedComponentsFunction, byte[]>(makeSlowSetConfigEncodedComponentsFunction, blockParameter);
        }

        public Task<byte[]> MakeTestnetSetConfigEncodedComponentsQueryAsync(MakeTestnetSetConfigEncodedComponentsFunction makeTestnetSetConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<MakeTestnetSetConfigEncodedComponentsFunction, byte[]>(makeTestnetSetConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> MakeTestnetSetConfigEncodedComponentsQueryAsync(ulong alphaPPB, byte rMax, List<byte> s, List<byte[]> offchainPublicKeys, string peerIDs, byte[] diffieHellmanPoint, byte[] sharedSecretHash, List<byte[]> encryptions, BlockParameter blockParameter = null)
        {
            var makeTestnetSetConfigEncodedComponentsFunction = new MakeTestnetSetConfigEncodedComponentsFunction();
                makeTestnetSetConfigEncodedComponentsFunction.AlphaPPB = alphaPPB;
                makeTestnetSetConfigEncodedComponentsFunction.RMax = rMax;
                makeTestnetSetConfigEncodedComponentsFunction.S = s;
                makeTestnetSetConfigEncodedComponentsFunction.OffchainPublicKeys = offchainPublicKeys;
                makeTestnetSetConfigEncodedComponentsFunction.PeerIDs = peerIDs;
                makeTestnetSetConfigEncodedComponentsFunction.DiffieHellmanPoint = diffieHellmanPoint;
                makeTestnetSetConfigEncodedComponentsFunction.SharedSecretHash = sharedSecretHash;
                makeTestnetSetConfigEncodedComponentsFunction.Encryptions = encryptions;
            
            return ContractHandler.QueryAsync<MakeTestnetSetConfigEncodedComponentsFunction, byte[]>(makeTestnetSetConfigEncodedComponentsFunction, blockParameter);
        }

        public Task<List<ulong>> PackDeltaComponentQueryAsync(PackDeltaComponentFunction packDeltaComponentFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PackDeltaComponentFunction, List<ulong>>(packDeltaComponentFunction, blockParameter);
        }

        
        public Task<List<ulong>> PackDeltaComponentQueryAsync(ulong deltaProgressNS, ulong deltaResendNS, ulong deltaRoundNS, ulong deltaGraceNS, ulong deltaC, ulong alphaPPB, ulong deltaStage, BlockParameter blockParameter = null)
        {
            var packDeltaComponentFunction = new PackDeltaComponentFunction();
                packDeltaComponentFunction.DeltaProgressNS = deltaProgressNS;
                packDeltaComponentFunction.DeltaResendNS = deltaResendNS;
                packDeltaComponentFunction.DeltaRoundNS = deltaRoundNS;
                packDeltaComponentFunction.DeltaGraceNS = deltaGraceNS;
                packDeltaComponentFunction.DeltaC = deltaC;
                packDeltaComponentFunction.AlphaPPB = alphaPPB;
                packDeltaComponentFunction.DeltaStage = deltaStage;
            
            return ContractHandler.QueryAsync<PackDeltaComponentFunction, List<ulong>>(packDeltaComponentFunction, blockParameter);
        }

        public Task<byte[]> SetConfigEncodedComponentsQueryAsync(SetConfigEncodedComponentsFunction setConfigEncodedComponentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SetConfigEncodedComponentsFunction, byte[]>(setConfigEncodedComponentsFunction, blockParameter);
        }

        
        public Task<byte[]> SetConfigEncodedComponentsQueryAsync(SetConfigEncodedComponents components, BlockParameter blockParameter = null)
        {
            var setConfigEncodedComponentsFunction = new SetConfigEncodedComponentsFunction();
                setConfigEncodedComponentsFunction.Components = components;
            
            return ContractHandler.QueryAsync<SetConfigEncodedComponentsFunction, byte[]>(setConfigEncodedComponentsFunction, blockParameter);
        }
    }
}
