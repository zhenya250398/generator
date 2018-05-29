using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GeneratorLicense
{
    public class GeneratorManager
    {
        public void GenerateNewKeyPair()
        {
            string withSecret;
            string woSecret;
            using (var rsaCsp = new RSACryptoServiceProvider())
            {
                withSecret = rsaCsp.ToXmlString(true);
                woSecret = rsaCsp.ToXmlString(false);
            }

            File.WriteAllText("private.xml", withSecret);
            File.WriteAllText("public.xml", woSecret);
        }

        public void CreateLicenseFile(LicenseDto dto, string fileName)
        {
            var ms = new MemoryStream();
            new XmlSerializer(typeof(LicenseDto)).Serialize(ms, dto);

            // Create a new CspParameters object to specify
            // a key container.

            string secretKey = File.ReadAllText("private.xml");

            // Create a new RSA signing key and save it in the container. 
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
            rsaKey.FromXmlString(secretKey);

            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();

            // Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = true;
            ms.Seek(0, SeekOrigin.Begin);
            xmlDoc.Load(ms);

            // Sign the XML document. 
            SignXml(xmlDoc, rsaKey);

            // Save the document.
            xmlDoc.Save(fileName);

        }

        // Sign an XML file. 
        // This document cannot be verified unless the verifying 
        // code has the key with which it was signed.
        public static void SignXml(XmlDocument xmlDoc, RSA Key)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = Key;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

        }
    }
}
