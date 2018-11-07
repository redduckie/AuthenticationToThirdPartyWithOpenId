using System.Security.Cryptography.X509Certificates;

namespace X509Helper
{
    public static class X509
    {
        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certCollection.Count > 0) return certCollection[0];
            }
            return null;
        }
    }
}
