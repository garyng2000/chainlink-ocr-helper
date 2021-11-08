using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Chainlink.OCRConfig.ContractDefinition
{
    public partial class SetConfigEncodedComponents : SetConfigEncodedComponentsBase { }

    public class SetConfigEncodedComponentsBase 
    {
        [Parameter("uint64", "deltaProgress", 1)]
        public virtual ulong DeltaProgress { get; set; }
        [Parameter("uint64", "deltaResend", 2)]
        public virtual ulong DeltaResend { get; set; }
        [Parameter("uint64", "deltaRound", 3)]
        public virtual ulong DeltaRound { get; set; }
        [Parameter("uint64", "deltaGrace", 4)]
        public virtual ulong DeltaGrace { get; set; }
        [Parameter("uint64", "deltaC", 5)]
        public virtual ulong DeltaC { get; set; }
        [Parameter("uint64", "alphaPPB", 6)]
        public virtual ulong AlphaPPB { get; set; }
        [Parameter("uint64", "deltaStage", 7)]
        public virtual ulong DeltaStage { get; set; }
        [Parameter("uint8", "rMax", 8)]
        public virtual byte RMax { get; set; }
        [Parameter("uint8[]", "s", 9)]
        public virtual List<byte> S { get; set; }
        [Parameter("bytes32[]", "offchainPublicKeys", 10)]
        public virtual List<byte[]> OffchainPublicKeys { get; set; }
        [Parameter("string", "peerIDs", 11)]
        public virtual string PeerIDs { get; set; }
        [Parameter("tuple", "sharedSecretEncryptions", 12)]
        public virtual SharedSecretEncryptions SharedSecretEncryptions { get; set; }
    }
}
