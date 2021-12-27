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
        Testnet,
        OnDemand,
        Mainnet
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
        public static Regex strip0x = new Regex("^0[xX]");
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
                    hex = strip0x.Replace(hex.ToLower(),"");
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
            var @string = "0x" + BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
            serializer.Serialize(writer, @string);
        }
    }
    public class OCROracleIdentity
    {
        static readonly Regex rx_strip_prefix = new Regex("(^[^_]*_?)?(.*)");
        public string PeerID;
        public byte[] OffchainPublicKey;
        public byte[] OnChainSigningAddress;
        public byte[] TransmitAddress;
        public byte[] SharedSecretEncryptionPublicKey;

        public static string strip_prefix(string s)
        {
            return rx_strip_prefix.Replace(s, m => {
                return !string.IsNullOrEmpty(m.Groups[2].Value) ? m.Groups[2].Value : m.Groups[1].Value;
                });
        }
        public OCROracleIdentity() { }
        public OCROracleIdentity(string peerId, byte[] offchainPublicKey, byte[] onchainSigningAddress, byte[] transmitAddress, byte[] sharedSecretEncryptionPublicKey)
        {
            if (string.IsNullOrEmpty(peerId)) throw new Exception("peerId must be the base58 p2p address");
            if (offchainPublicKey == null || offchainPublicKey.Length != 32) throw new Exception("offchainPublicKey must be a 32 byte x25519 public key");
            if (onchainSigningAddress == null || onchainSigningAddress.Length != 20) throw new Exception("onchainSigningAddress must be a 20 byte eth address");
            if (transmitAddress == null || transmitAddress.Length != 20) throw new Exception("transmitAddress must be a 20 byte eth address");
            if (sharedSecretEncryptionPublicKey == null || sharedSecretEncryptionPublicKey.Length != 32) throw new Exception("sharedSecretEncryptionPublicKey must be a 32 byte x25519 public key");
            PeerID = strip_prefix(peerId).Trim();
            OffchainPublicKey = offchainPublicKey;
            OnChainSigningAddress = onchainSigningAddress;
            TransmitAddress = transmitAddress;
            SharedSecretEncryptionPublicKey = sharedSecretEncryptionPublicKey;
        }

        public static OCROracleIdentity MakeOCROracleIdentity(string peerId, string offchainPublicKey, string onchainSigningAddress, string transmitAddress, string sharedSecretEncryptionPublicKey)
        {
            if (string.IsNullOrEmpty(peerId)) throw new Exception("peerId must be the base58 p2p address(");
            if (string.IsNullOrEmpty(offchainPublicKey)) throw new Exception("offchainPublicKey must be a x25519 public key");
            if (string.IsNullOrEmpty(onchainSigningAddress)) throw new Exception("onchainSigningAddress must be a 40 char eth address");
            if (string.IsNullOrEmpty(transmitAddress)) throw new Exception("transmitAddress must be a 40 char eth address");
            if (string.IsNullOrEmpty(sharedSecretEncryptionPublicKey)) throw new Exception("sharedSecretEncryptionPublicKey must be a x25519 public key");
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
                    "p2p_12D3KooWBwTUagTaa6Xmrw3UxSdKamrUhCBm5jrQSC4hH4kTCAjE",
                    "ocroff_e8633419d06a8c838b0d03364dfccdf91fda2516400a27ffdd8d3a42606bafd3",
                    "ocrsad_0x00a22bb77b6135e1faf5606e8ae9eb1029e59356",
                    "0x25624657239041F7f28a314548E63ae889F960e2",
                    "ocroff_e8633419d06a8c838b0d03364dfccdf91fda2516400a27ffdd8d3a42606bafd3"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWQkerdyk9oRFxi9SKGzHbECCiFvej4mCF4tTVDNs4FCEK",
                    "ocroff_435c6a7a815cb0b5876248e961a138815d2611143ef18cbf371de32c03248edb",
                    "ocrsad_0x914f2fa882025f9acef9708c5f518c10a6b9e480",
                    "0x2b5BA682F9204d089449EC0E9de55389DE9691A6",
                    "ocrcfg_d4d0eea9e61bdcce89656864c9b213158cfcc86288d1eaed35e94a955abdea71"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWD6bdJcoNCPGrAQuZbNmWD6Lwh6jbokibYjd9Rvrrkvmj",
                    "ocroff_7fafaa5227956d3a54ea58bc48168c9d58956aba100de2225287cf4dcbcf4906",
                    "ocrsad_0x0882e9ab349ac4b3f593a310250bccc23e3a8998",
                    "0xd3A10D5cc5a03D5568CF787DDB48BE0E36D9A006",
                    "ocrcfg_314dba470e37a1c221289812de9016881d101ea60848c0b61b7790f8d529ec38"),
                OCROracleIdentity.MakeOCROracleIdentity(
                    "p2p_12D3KooWB8s5K6edpULGsiekeAJDTMX6jqfzQ5aF4ThP4yMFpRCm",
                    "ocroff_3bb88f0dca0049f59f425dbd38e9ad3bc03c7ce00fc948c152fa0c8a848f7eba",
                    "ocrsad_0x2d421432b37220f910b9dbf212096f5c6d188c40",
                    "0x4fC8bFaDbF9B1D40c2D82a06c2cCA2e21867B7d4",
                    "ocrcfg_144194337cdd8886f061fbc2b3914178fef6cff925ba6f2a27881ccf4b9e2d11"),

            };
        }
        public static OCRNetworkParams[] ocrNetworkParams = new OCRNetworkParams[] {
        /* chainlink used default: 35, 17, 30, 12(first 4, mainly retry related for each round, i.e. transmit), 
           should never > deltaC but has must >= limit set in libocr depending on network
           the default is generally fine so the min deltaC is 60s
           deltaC(control transmit frequency) and deltaStage controls frequency between submission 
           alphaPPB(%) controls 'rate change submit', in 1e9 so 1% is 1e7
         */

            // slow
            new OCRNetworkParams(){
                DeltaProgress = 35 * secondInNS,
                DeltaResend = 17 * secondInNS,
                DeltaRound = 30 * secondInNS,
                DeltaGrace = 12 * secondInNS,
                DeltaC = 20 * 60 * secondInNS, 
                DeltaStage = 20 *secondInNS  },
            // moderate
            new OCRNetworkParams(){
                DeltaProgress = 35 * secondInNS,
                DeltaResend = 17 * secondInNS,
                DeltaRound = 30 * secondInNS,
                DeltaGrace = 12 * secondInNS,
                DeltaC = 10 * 60 * secondInNS, 
                DeltaStage = 10 *secondInNS  },
            // fast
            new OCRNetworkParams(){
                DeltaProgress = 35 * secondInNS,
                DeltaResend = 17 * secondInNS,
                DeltaRound = 30 * secondInNS,
                DeltaGrace = 12 * secondInNS,
                DeltaC = 1 * 60 * secondInNS, 
                DeltaStage = 5 *secondInNS  },
            // testnet
            new OCRNetworkParams(){
                DeltaProgress = 35 * secondInNS,
                DeltaResend = 17 * secondInNS,
                DeltaRound = 30 * secondInNS,
                DeltaGrace = 12 * secondInNS,
                DeltaC = 1 * 60 * secondInNS, 
                DeltaStage = 5 *secondInNS  },
            // super slow(on demand only)
            new OCRNetworkParams(){ 
                DeltaProgress = 35 * secondInNS, 
                DeltaResend = 17 * secondInNS, 
                DeltaRound = 30 * secondInNS, 
                DeltaGrace = 12 * secondInNS, 
                DeltaC = 60 * 60 * 24 * 3650 * 5 * secondInNS, 
                DeltaStage = 30 *secondInNS  },
           // default slow, mainnet and private unknown
            new OCRNetworkParams(){
                DeltaProgress = 35 * secondInNS,
                DeltaResend = 17 * secondInNS,
                DeltaRound = 30 * secondInNS,
                DeltaGrace = 12 * secondInNS,
                DeltaC = 60 * 60 * secondInNS,
                DeltaStage = 30 *secondInNS  },
        };
        protected static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static SharedSecretEncryptions EncryptSecretKey(byte[][] x25519ConfigPublicKeys, byte[] sharedSecret, byte[] ephemeralSecretKey)
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
            if (x25519ConfigPublicKeys == null || ephemeralSecretKey.Length == 0)
            {
                throw new Exception("x25519ConfigPublicKeys cannot be null or empty");
            }
            var keccak256 = new Sha3Keccack();
            var aesProvider = new AesCryptoServiceProvider() {
                BlockSize = keyBlockSize * 8,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                KeySize = keyBlockSize * 8
            };
            var ephemeralSecretKeyDhPoint = GoX25519.Curve25519.ScalarMultiplication(ephemeralSecretKey, GoX25519.Curve25519.Basepoint);
            var encryptedKeys = x25519ConfigPublicKeys.Select((x25519PublicKey, i) => {
                if (x25519PublicKey == null || x25519PublicKey.Length != 32)
                {
                    throw new Exception(string.Format("x25519ConfigPublicKey {0} must be 32 bytes in length", i));
                }
                var oracleDhPoint = GoX25519.Curve25519.ScalarMultiplication(ephemeralSecretKey, x25519PublicKey);
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
        public static SharedSecretEncryptions EncryptSecretKey(byte[][] x25519PublicKeys)
        {
            byte[] ephemeralSecretKey = new byte[32];
            byte[] sharedSecret = new byte[16];

            rng.GetBytes(ephemeralSecretKey);
            rng.GetBytes(sharedSecret);
            return EncryptSecretKey(x25519PublicKeys, sharedSecret, ephemeralSecretKey);
        }

        public static string ToJson(Object o)
        {
            var setting = new JsonSerializerSettings { Converters = { new BytesToHexConverter() } };
            var json = JsonConvert.SerializeObject(o, setting);
            return json;
        }
        public static T FromJson<T>(string s)
        {
            var setting = new JsonSerializerSettings { Converters = { new BytesToHexConverter() } };
            var o = JsonConvert.DeserializeObject<T>(s, setting);
            return o;
        }
        public static SetConfigEncodedComponents MakeSetConfigEncodedComponents(NetworkType networkType, List<OCROracleIdentity> oracles
            , UInt64 alphaPPB // rate change based triggering base 1e9 so 0.01(i.e. 1%) is 1e7
            , byte rMax
            , byte[] s // stake/weighting per node, default by chainlink is 1,2,2,...
            , string sharedSecret = null)
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
