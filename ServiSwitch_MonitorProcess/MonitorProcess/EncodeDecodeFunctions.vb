Imports System.Security.Cryptography
Imports System.Text
Imports System.IO
Imports System.Xml

Public Class EncodeDecodeFunctions
    Dim PathSecure As String = "C:\SystemATM\Ejecutables\SECURITY\security_file.xml"

    Dim plainText As String
    ' original plaintext
    ' Dim passPhrase As String = "Z12xc34v"
    'Dim passPhrase As String = "288745E85BBEE834E8907D83159A2327763EA3B22B72FB6A,D1EF1B835409B5AF"
    Dim passPhrase As String = String.Empty
    ' can be any string
    ' Dim saltValue As String = "Qaz13wsx24"
    'Dim saltValue As String = "D3879630E8E2E8D4E11AA9065C64C968E9115EF7198397F5,5D653E5ECEF080C6"
    Dim saltValue As String = String.Empty

    ' can be any string
    Dim hashAlgorithm As String = "SHA256"
    ' can be "MD5"
    Dim passwordIterations As Integer = 16
    ' can be any number
    ' Dim initVector As String = "@1B2c3D4e5F6g7H8"
    'Dim initVector As String = "CBEC42762D04A772"
    Dim initVector As String = String.Empty
    ' must be 16 bytes
    Dim keySize As Integer = 256
    ' can be 192 or 128
    Dim DecimalKey As String = String.Empty
    Dim EncryptConnString(2) As String

    Public Function Load_Config_Keys() As Byte
        Dim ErrorCode As Int16 = 1
        Dim NodeTemp As XmlNodeList
        Dim XmlConfigFile As New XmlDocument

        Try
            XmlConfigFile.Load(PathSecure)
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
            Return ErrorCode
        End Try

        '------------------------- INSTITUTION DEFINITIONS ------------------------------
        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_1")
        passPhrase = ProcessReverseLevelOne(NodeTemp.ItemOf(0).InnerText.ToString)

        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_2")
        saltValue = ProcessReverseLevelOne(NodeTemp.ItemOf(0).InnerText.ToString)

        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_3")
        initVector = ProcessReverseLevelOne(NodeTemp.ItemOf(0).InnerText.ToString)

        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_4")
        DecimalKey = ProcessReverseLevelOne(NodeTemp.ItemOf(0).InnerText.ToString)
        '*******************************************************************************
        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_5")
        EncryptConnString(0) = NodeTemp.ItemOf(0).InnerText.ToString

        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_6")
        EncryptConnString(1) = NodeTemp.ItemOf(0).InnerText.ToString

        NodeTemp = XmlConfigFile.GetElementsByTagName("secure_7")
        EncryptConnString(2) = NodeTemp.ItemOf(0).InnerText.ToString

        ErrorCode = 0
        Return ErrorCode

    End Function

    'Public Shared Function Encrypt(plainText As String, passPhrase As String, saltValue As String, hashAlgorithm As String, passwordIterations As Integer, initVector As String, keySize As Integer) As String
    Public Function Encrypt(plainText As String) As String
        ' Convert strings into byte arrays.
        ' Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8 
        ' encoding.
        Dim initVectorBytes As Byte() = Encoding.ASCII.GetBytes(initVector)
        Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(saltValue)

        ' Convert our plaintext into a byte array.
        ' Let us assume that plaintext contains UTF8-encoded characters.
        Dim plainTextBytes As Byte() = Encoding.UTF8.GetBytes(plainText)

        ' First, we must create a password, from which the key will be derived.
        ' This password will be generated from the specified passphrase and 
        ' salt value. The password will be created using the specified hash 
        ' algorithm. Password creation can be done in several iterations.
        'Dim password As New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)
        Dim password As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As New RijndaelManaged()

        ' It is reasonable to set encryption mode to Cipher Block Chaining
        ' (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate encryptor from the existing key bytes and initialization 
        ' vector. Key size will be defined based on the number of the key 
        ' bytes.
        Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As New MemoryStream()

        ' Define cryptographic stream (always use Write mode for encryption).
        Dim cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
        ' Start encrypting.
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)

        ' Finish encrypting.
        cryptoStream.FlushFinalBlock()

        ' Convert our encrypted data from a memory stream into a byte array.
        Dim cipherTextBytes As Byte() = memoryStream.ToArray()

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()

        ' Convert encrypted data into a base64-encoded string.
        Dim cipherText As String = Convert.ToBase64String(cipherTextBytes)

        ' Return encrypted string.
        Return cipherText
    End Function

    'Public Shared Function Decrypt(cipherText As String, passPhrase As String, saltValue As String, hashAlgorithm As String, passwordIterations As Integer, initVector As String, keySize As Integer) As String
    Public Function Decrypt(cipherText As String) As String
        ' Convert strings defining encryption key characteristics into byte
        ' arrays. Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8
        ' encoding.
        Dim initVectorBytes As Byte() = Encoding.ASCII.GetBytes(initVector)
        Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(saltValue)

        ' Convert our ciphertext into a byte array.
        Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipherText)

        ' First, we must create a password, from which the key will be 
        ' derived. This password will be generated from the specified 
        ' passphrase and salt value. The password will be created using
        ' the specified hash algorithm. Password creation can be done in
        ' several iterations.
        'Dim password As New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)
        Dim password As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As New RijndaelManaged()

        ' It is reasonable to set encryption mode to Cipher Block Chaining
        ' (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate decryptor from the existing key bytes and initialization 
        ' vector. Key size will be defined based on the number of the key 
        ' bytes.
        Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As New MemoryStream(cipherTextBytes)

        ' Define cryptographic stream (always use Read mode for encryption).
        Dim cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)

        ' Since at this point we don't know what the size of decrypted data
        ' will be, allocate the buffer long enough to hold ciphertext;
        ' plaintext is never longer than ciphertext.
        Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}

        ' Start decrypting.
        Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()

        ' Convert decrypted data into a string. 
        ' Let us assume that the original plaintext string was UTF8-encoded.
        Dim plainText As String = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)

        ' Return decrypted string.   
        Return plainText
    End Function

    Public Function ProcessApplyLevelOne(ByVal InputData As String) As String
        Dim StringOut As String = String.Empty

        Try
            Dim ClearBytes() As Byte = System.Text.Encoding.ASCII.GetBytes(InputData)
            Array.Reverse(ClearBytes)
            Dim ClearBitString As String = BitConverter.ToString(ClearBytes)
            StringOut = ClearBitString
        Catch ex As Exception
            StringOut = "X X X X X X X X X X"
        End Try

        Return StringOut

    End Function

    Public Function ProcessReverseLevelOne(ByVal InputData As String) As String
        Dim StringOut As String = String.Empty

        Try
            Dim BitMatrix As String() = InputData.Split(New String() {"-"}, StringSplitOptions.RemoveEmptyEntries)
            Dim CipherBytes(UBound(BitMatrix)) As Byte
            For x As Int16 = 0 To UBound(BitMatrix)
                CipherBytes(x) = Convert.ToInt32(BitMatrix(x), 16)
            Next
            Array.Reverse(CipherBytes)
            Dim CipherString As String = System.Text.Encoding.ASCII.GetString(CipherBytes)
            StringOut = CipherString
        Catch ex As Exception
            StringOut = "X X X X X X X X X X"
        End Try

        Return StringOut

    End Function

    Public Function SetgetStringConnection(ByVal Idx As Byte) As String

        EncryptConnString(Idx) = Decrypt(EncryptConnString(Idx))
        Return EncryptConnString(Idx)

    End Function

End Class
