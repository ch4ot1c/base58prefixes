
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Test out pairs of version bytes (Z forks, I'm looking at you)
public class Program
{
  public static void Main(string[] args)
  {
    StringBuilder sb = new StringBuilder();
    /*for (int w = 160; w < 520; w+=8)
    {*/

    //t1 (160)
    //var a = 0x1C;
    //var b = 0xB8;

    sb.AppendFormat("Version [{0},{1}] - {2}", a, b, GenerateZeroAddress((byte)a, (byte)b, 160));
    sb.AppendLine();
    /*}*/
    string s=sb.ToString();
    Console.WriteLine(s);
  }
  public static string GenerateZeroAddress(byte versionByte1, byte versionByte2, int width)
  {
    //var width = 160 for pubkey/script t1/t3; 512 for zc
    var hash=new byte[width/8]; //in a normal address, this would be a ripemd160(publickey)
    var bytes=new byte[width/8+2+4]; //include space for 2 version bytes and 4 checksum bytes
    //hash is all 0s right now (dummy value)
    //hash.CopyTo(bytes, 2); //not necessary for our case, but just for illustration
    bytes[0]=versionByte1;
    bytes[1]=versionByte2;

    using(var sha256=new SHA256Managed())
    {
      //include version, but do not include leading 4 bytes where checksum will go
      var tmp = sha256.ComputeHash(sha256.ComputeHash(bytes.Take(width/8+2).ToArray()));
      for (int i = 0; i < 4; i++)
      {
        //take 4 bytes and add them to the end of the address bytes. this is our checksum
        bytes[bytes.Length - 4 + i] = tmp[i];
      }
    }
    //base58 it up and it's all good
    return Base58Encode(bytes);
  }


  public static string Base58Encode(byte[] array)
  {
    const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    string retString = string.Empty;
    BigInteger encodeSize = ALPHABET.Length;
    BigInteger arrayToInt = 0;
    for (int i = 0; i < array.Length; ++i)
    {
      arrayToInt = arrayToInt * 256 + array[i];
    }
    while (arrayToInt > 0)
    {
      int rem = (int)(arrayToInt % encodeSize);
      arrayToInt /= encodeSize;
      retString = ALPHABET[rem] + retString;
    }
    for (int i = 0; i < array.Length && array[i] == 0; ++i)
    retString = ALPHABET[0] + retString;

    return retString;
  }
}
