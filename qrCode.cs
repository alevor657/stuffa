using System;
using System.Net;
using QRCoder;


//get local IP (IPV4 address)
public class IpV4Adress
{
    public static void GetIp()
    {
        try
        {
            //get name of host
            string hostName = Dns.GetHostName();
            Console.WriteLine(hostName);

            //get the Ip address
            string localIp = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Console.WriteLine("Ip address is : " + localIp);
            return localIp;
        }
        catch
        {
            string errorMessageIp = "could not retrieve local IP";
            Console.WriteLine(errorMessageIp);
            throw;
        }
    }
}

//Use Ip address to create an QR code
public class QrCode
{
    public static void GetQR()
    {
        try
        {
            //get Ip address to use in the QR code
            string ipAddress = IpV4Adress.GetIp();

            //create instance of the generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            //create the data object with the IP
            // ECCLevel (Error correction level:  L(7%), M(15%), Q(25%), H(30%) : how much of the QR code can be hidden and still work)
            QRCodeData qrCodeData = qrGenerator.createQrCode(ipAddress, QrCodeGenerator.ECCLevel.Q);

            //create QR code object
            QRCode qrCode = new QRCode(qrCodeData);

            //render QR code as bitmap and display
            Bitmap CodeImage = qrCode.GetGraphic(20);
        }
        catch
        {
            string errorMessageQr = "could not show QR code";
            Console.WriteLine(errorMessageQr);
            throw;
        }
    }
}
