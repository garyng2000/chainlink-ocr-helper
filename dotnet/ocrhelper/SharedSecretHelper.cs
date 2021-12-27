using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;
using Chainlink.OCRConfig.ContractDefinition;
using GoX25519;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Chainlink.OCRConfig
{
    class SharedSecretHelper
    {
        static readonly Regex rx_strip_prefix = new Regex("(^[^_]*_?)(.*)");
        protected static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static string strip_prefix(string s)
        {
            return rx_strip_prefix.Replace(s, m => m.Groups[2].Value);
        }
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
            var aesProvider = new AesCryptoServiceProvider()
            {
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
            return new SharedSecretEncryptions()
            {
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

        public static SharedSecretEncryptions EncryptSecretKey(string[] x25519PublicKeys)
        {
            byte[] ephemeralSecretKey = new byte[32];
            byte[] sharedSecret = new byte[16];

            rng.GetBytes(ephemeralSecretKey);
            rng.GetBytes(sharedSecret);
            return EncryptSecretKey(x25519PublicKeys.Select(pk=> strip_prefix(pk).Trim().HexToByteArray()).ToArray(), sharedSecret, ephemeralSecretKey);
        }

    }
}
