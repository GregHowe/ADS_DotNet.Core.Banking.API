namespace Common{
    using System.Security.Cryptography;
    using System.Text;
    using System;
    public class Hash{

    public static string getHash(string text)  
    {  
        using(var sha256 = SHA256.Create())  
        {  
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text)); 
            var result = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            Console.WriteLine(result);
            return result;   
        }  
    }  

    }
}