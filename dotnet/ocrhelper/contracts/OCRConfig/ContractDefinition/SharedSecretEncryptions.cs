using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Chainlink.OCRConfig.ContractDefinition
{
    public partial class SharedSecretEncryptions : SharedSecretEncryptionsBase { }

    public class SharedSecretEncryptionsBase 
    {
        [Parameter("bytes32", "diffieHellmanPoint", 1)]
        public virtual byte[] DiffieHellmanPoint { get; set; }
        [Parameter("bytes32", "sharedSecretHash", 2)]
        public virtual byte[] SharedSecretHash { get; set; }
        [Parameter("bytes16[]", "encryptions", 3)]
        public virtual List<byte[]> Encryptions { get; set; }
    }
}
