using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Chainlink.OCRConfig.ContractDefinition;
using GoX25519;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;

namespace Chainlink.OCRConfig
{
    public enum NetworkType
    {
        Slow = 0,
        Moderate,
        Fast,
        Testnet
    }
    public static class MyExtensions
    {
        public static string ToJsonHex(this Object o)
        {
            var setting = new JsonSerializerSettings { Converters = { new BytesToHexConverter() } };
            var json = JsonConvert.SerializeObject(o, setting);
            return json;
        }
    }
    public class BytesToHexConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var hex = serializer.Deserialize<string>(reader);
                if (!string.IsNullOrEmpty(hex))
                {
                    return Enumerable.Range(0, hex.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                            .ToArray();
                }
            }
            return Enumerable.Empty<byte>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bytes = value as byte[];
            var @string = BitConverter.ToString(bytes).Replace("-", string.Empty);
            serializer.Serialize(writer, @string);
        }
    }
    public class OCROracleIdentity
    {
        static readonly Regex rx_strip_prefix = new Regex("(^[^_]*_?)(.*)");
        public string PeerID;
        public byte[] OffchainPublicKey;
        public byte[] OnChainSigningAddress;
        public byte[] TransmitAddress;
        public byte[] SharedSecretEncryptionPublicKey;

        public static string strip_prefix(string s)
        {
            return rx_strip_prefix.Replace(s, m => m.Groups[2].Value);
        }
        public OCROracleIdentity() { }
        public OCROracleIdentity(string peerId, byte[] offchainPublicKey, byte[] onchainSigningAddress, byte[] transmitAddress, byte[] sharedSecretEncryptionPublicKey)
        {
            if (string.IsNullOrEmpty(peerId)) throw new Exception("peerId must be the base58 p2p address");
            if (offchainPublicKey == null || offchainPublicKey.Length != 32) throw new Exception("offchainPublicKey must be a 32 byte ed25519 public key");
            if (onchainSigningAddress == null || onchainSigningAddress.Length != 20) throw new Exception("onchainSigningAddress must be a 20 byte eth address");
            if (transmitAddress == null || transmitAddress.Length != 20) throw new Exception("transmitAddress must be a 20 byte eth address");
            if (sharedSecretEncryptionPublicKey == null || sharedSecretEncryptionPublicKey.Length != 32) throw new Exception("sharedSecretEncryptionPublicKey must be a 32 byte ed25519 public key");
            PeerID = strip_prefix(peerId).Trim();
            OffchainPublicKey = offchainPublicKey;
            OnChainSigningAddress = onchainSigningAddress;
            TransmitAddress = transmitAddress;
            SharedSecretEncryptionPublicKey = sharedSecretEncryptionPublicKey;
        }

        public static OCROracleIdentity MakeOCROracleIdentity(string peerId, string offchainPublicKey, string onchainSigningAddress, string transmitAddress, string sharedSecretEncryptionPublicKey)
        {
            if (string.IsNullOrEmpty(peerId)) throw new Exception("peerId must be the base58 p2p address(");
            if (string.IsNullOrEmpty(offchainPublicKey)) throw new Exception("offchainPublicKey must be a ed25519 public key");
            if (string.IsNullOrEmpty(onchainSigningAddress)) throw new Exception("onchainSigningAddress must be a 40 char eth address");
            if (string.IsNullOrEmpty(transmitAddress)) throw new Exception("transmitAddress must be a 40 char eth address");
            if (string.IsNullOrEmpty(sharedSecretEncryptionPublicKey)) throw new Exception("sharedSecretEncryptionPublicKey must be a ed25519 public key");
            return new OCROracleIdentity(
                strip_prefix(peerId).Trim(),
                strip_prefix(offchainPublicKey).Trim().HexToByteArray(),
                strip_prefix(onchainSigningAddress).Trim().HexToByteArray(),
                strip_prefix(transmitAddress).Trim().HexToByteArray(),
                strip_prefix(sharedSecretEncryptionPublicKey).Trim().HexToByteArray());
        }

    }

    public class OCRNetworkParams
    {
        public virtual UInt64 DeltaProgress { get; set; }
        public virtual UInt64 DeltaResend { get; set; }
        public virtual UInt64 DeltaRound { get; set; }
        public virtual UInt64 DeltaGrace { get; set; }
        public virtual UInt64 DeltaC { get; set; }
        public virtual UInt64 AlphaPPB { get; set; }
        public virtual UInt64 DeltaStage { get; set; }
    }
    public class ChainlinkOCR
    {

        private const UInt64 secondInNS = 1000000000;
        public static List<OCROracleIdentity> TestOracles() {
            return new List<OCROracleIdentity>()
            {
                // this is a sample, it doesn't work
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWAjckJmmuNuWQdb6gFwYJ4ksCxnuPj2RnoBw2vENt4HBR", 
                    "ocroff_8f81e68a89546235a1472f3cb22956c27ae067cf305ab83009c19b632757f3ae",
                    "ocrsad_0x7244dfbf59d40f18ac501193eaab04d33e7bfbda",
                    "0x858df45E37352d2606AF9923757Fd809Cdd22037",
                    "ocrcfg_89166bf5205dcf375a64ee3917f894a74a54d4dd630013fd8f26ac0bc069ae02"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWAjckJmmuNuWQdb6gFwYJ4ksCxnuPj2RnoBw2vENt4HBR", 
                    "ocroff_8f81e68a89546235a1472f3cb22956c27ae067cf305ab83009c19b632757f3ae",
                    "ocrsad_0x7244dfbf59d40f18ac501193eaab04d33e7bfbda",
                    "0x858df45E37352d2606AF9923757Fd809Cdd22037",
                    "ocrcfg_89166bf5205dcf375a64ee3917f894a74a54d4dd630013fd8f26ac0bc069ae02"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWAjckJmmuNuWQdb6gFwYJ4ksCxnuPj2RnoBw2vENt4HBR", 
                    "ocroff_8f81e68a89546235a1472f3cb22956c27ae067cf305ab83009c19b632757f3ae",
                    "ocrsad_0x7244dfbf59d40f18ac501193eaab04d33e7bfbda",
                    "0x858df45E37352d2606AF9923757Fd809Cdd22037",
                    "ocrcfg_89166bf5205dcf375a64ee3917f894a74a54d4dd630013fd8f26ac0bc069ae02"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWAjckJmmuNuWQdb6gFwYJ4ksCxnuPj2RnoBw2vENt4HBR", 
                    "ocroff_8f81e68a89546235a1472f3cb22956c27ae067cf305ab83009c19b632757f3ae",
                    "ocrsad_0x7244dfbf59d40f18ac501193eaab04d33e7bfbda",
                    "0x858df45E37352d2606AF9923757Fd809Cdd22037",
                    "ocrcfg_89166bf5205dcf375a64ee3917f894a74a54d4dd630013fd8f26ac0bc069ae02"),

            };
        }
        public static OCRNetworkParams[] ocrNetworkParams = new OCRNetworkParams[4] {
            // slow
            new OCRNetworkParams(){ DeltaProgress = 23 * secondInNS, DeltaResend = 10 * secondInNS, DeltaRound = 20 *secondInNS, DeltaGrace = 15 * secondInNS, DeltaC = 10 * 60 * secondInNS, DeltaStage = 20 *secondInNS  },
            // moderate
            new OCRNetworkParams(){ DeltaProgress = 23 * secondInNS, DeltaResend = 10 * secondInNS, DeltaRound = 20 *secondInNS, DeltaGrace = 15 * secondInNS, DeltaC = 1 * 60 * secondInNS, DeltaStage = 5 *secondInNS  },
            // fast
            new OCRNetworkParams(){ DeltaProgress = 8 * secondInNS, DeltaResend = 5 * secondInNS, DeltaRound = 5 *secondInNS, DeltaGrace = 3 * secondInNS, DeltaC = 10 * secondInNS, DeltaStage = 5 *secondInNS  },
            // testnet
            new OCRNetworkParams(){ DeltaProgress = 2 * secondInNS, DeltaResend = 2 * secondInNS, DeltaRound = 1 * secondInNS, DeltaGrace = 1 * secondInNS / 2, DeltaC = 1 * secondInNS, DeltaStage = 5 *secondInNS  },
        };
        protected static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static SharedSecretEncryptions EncryptSecretKey(byte[][] ed25519ConfigPublicKeys, byte[] sharedSecret, byte[] ephemeralSecretKey)
        {
            const int keyBlockSize = 16;
            if (sharedSecret == null || sharedSecret.Length != 16)
            {
                throw new Exception("sharedSecret must be 16 bytes in length");
            }
            if (ephemeralSecretKey == null || ephemeralSecretKey.Length != 32)
            {
                throw new Exception("ephemeralSecretKey must be 32 bytes in length");
            }
            if (ed25519ConfigPublicKeys == null || ephemeralSecretKey.Length == 0)
            {
                throw new Exception("ed25519ConfigPublicKeys cannot be null or empty");
            }
            var keccak256 = new Sha3Keccack();
            var aesProvider = new AesCryptoServiceProvider() {
                BlockSize = keyBlockSize * 8,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                KeySize = keyBlockSize * 8
            };
            var ephemeralSecretKeyDhPoint = GoX25519.Curve25519.ScalarMultiplication(ephemeralSecretKey, GoX25519.Curve25519.Basepoint);
            var encryptedKeys = ed25519ConfigPublicKeys.Select((ed25519PublicKey, i) => {
                if (ed25519PublicKey == null || ed25519PublicKey.Length != 32)
                {
                    throw new Exception(string.Format("ed25519ConfigPublicKey {0} must be 32 bytes in length", i));
                }
                var oracleDhPoint = GoX25519.Curve25519.ScalarMultiplication(ephemeralSecretKey, ed25519PublicKey);
                var key = keccak256.CalculateHash(oracleDhPoint).Take(keyBlockSize).ToArray();
                var aesEncryptor = aesProvider.CreateEncryptor(key, new byte[keyBlockSize] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                var encryptedKey = aesEncryptor.TransformFinalBlock(sharedSecret, 0, sharedSecret.Length);
                return encryptedKey;
            }).ToList();
            return new SharedSecretEncryptions() {
                 DiffieHellmanPoint = ephemeralSecretKeyDhPoint,
                 SharedSecretHash = keccak256.CalculateHash(sharedSecret),
                 Encryptions = encryptedKeys

            };
        }
        public static SharedSecretEncryptions EncryptSecretKey(byte[][] ed25519PublicKeys)
        {
            byte[] ephemeralSecretKey = new byte[32];
            byte[] sharedSecret = new byte[16];

            rng.GetBytes(ephemeralSecretKey);
            rng.GetBytes(sharedSecret);
            return EncryptSecretKey(ed25519PublicKeys, sharedSecret, ephemeralSecretKey);
        }

        public static string ToJson(Object o)
        {
            var setting = new JsonSerializerSettings { Converters = { new BytesToHexConverter() } };
            var json = JsonConvert.SerializeObject(o, setting);
            return json;
        }
        public static SetConfigEncodedComponents MakeSetConfigEncodedComponents(NetworkType networkType, List<OCROracleIdentity> oracles, UInt64 alphaPPB, byte rMax, byte[] s, string sharedSecret = null)
        {
            byte[] ephemeralSecretKey = new byte[32];
            byte[] _sharedSecret = null;

            if (string.IsNullOrEmpty(sharedSecret)) {
                _sharedSecret = new byte[16];
                rng.GetBytes(_sharedSecret);
            }
            else {
                if (sharedSecret.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) {
                    _sharedSecret = sharedSecret.HexToByteArray();
                }
                else {
                    _sharedSecret = System.Text.UTF8Encoding.UTF8.GetBytes(sharedSecret);
                }
                if (_sharedSecret.Length < 16) {
                    throw new Exception("sharedSecret must be either 16 char string or hex string of 32 char(with 0x prefix)");
                }
            };

            rng.GetBytes(ephemeralSecretKey);

            SharedSecretEncryptions sharedSecretEncryptions = EncryptSecretKey(oracles.Select(o => o.SharedSecretEncryptionPublicKey).ToArray(), _sharedSecret, ephemeralSecretKey);
            OCRNetworkParams networkParams = (int) networkType > ocrNetworkParams.Length ? ocrNetworkParams[0] : ocrNetworkParams[(int) networkType];
            SetConfigEncodedComponents setConfigEncodedComponents = new SetConfigEncodedComponents()
            {
                DeltaProgress = networkParams.DeltaProgress,
                DeltaResend = networkParams.DeltaResend,
                DeltaRound = networkParams.DeltaRound,
                DeltaGrace = networkParams.DeltaGrace,
                DeltaC = networkParams.DeltaC,
                AlphaPPB = alphaPPB,
                DeltaStage = networkParams.DeltaStage,
                RMax = rMax,
                S = s.ToList(),
                OffchainPublicKeys = oracles.Select(o => o.OffchainPublicKey).ToList(),
                PeerIDs = string.Join(",", oracles.Select(o => o.PeerID).ToArray()),
                SharedSecretEncryptions = sharedSecretEncryptions
            };
            return setConfigEncodedComponents;
        }
        public static string AbiEncodeSetConfigEncodedComponents(SetConfigEncodedComponents setConfigEncodedComponents)
        {
            Nethereum.Contracts.FunctionBuilder<SetConfigEncodedComponents> functionBuilder = new Nethereum.Contracts.FunctionBuilder<SetConfigEncodedComponents>("");
            var callInput = functionBuilder.CreateCallInput(setConfigEncodedComponents);
            return callInput.Data;
        }
    }

}
