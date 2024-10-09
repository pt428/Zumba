	using QRCoder;
	using System.Drawing;
 
namespace Zumba.Services
{

	public class QrCodeService
	{
        public Bitmap GenerateQrCode(string text)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode =new QRCode(qrCodeData)) 
                    {
                        return qrCode.GetGraphic(20);
                    }
                }
            }
        }

    }

}
