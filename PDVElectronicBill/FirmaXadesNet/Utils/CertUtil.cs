using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FirmaXadesNet.Utils
{
    public class CertUtil
    {
        #region Public methods

        public static X509Chain GetCertChain(X509Certificate2 certificate, X509Certificate2[] certificates = null)
        {
            X509Chain chain = new X509Chain();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;

            if (certificates != null)
            {
                chain.ChainPolicy.ExtraStore.AddRange(certificates);
            }

            if (!chain.Build(certificate))
            {
			    //TODO: Localize
			    throw new Exception("Can not build certification chain");
                // throw new Exception("No se puede construir la cadena de certificación");
            }

            return chain;
        }

        /// <summary>
        /// Selecciona un certificado del almacén de certificados
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 SelectCertificate(string message = null, string title = null)
        {
            X509Certificate2 cert = null;

            try
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection cers = store.Certificates.Find(X509FindType.FindBySubjectName, "My Cert's Subject Name", false);

                if (cers.Count>0)
                {
                    if (cert.HasPrivateKey == false)
                    {
                        throw new Exception("El certificado no tiene asociada una clave privada.");
                    }

                    cert = cers[0];
                };
                store.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("No se ha podido obtener la clave privada.", ex);
            }

            return cert;
        }
        //TODO: da Verificare Nuova Funzione da Fork
        /// <summary>
		/// Validates certificate chain, with manual validation of the rot certificate (passed as parameter).
        /// Caller must validate that the root is correct (i.e. look it up in a database).
        /// </summary>
        /// <param name="certificateToValidate">The certificate to be validated.</param>
        /// <param name="rootCertificate">The certificate chain should terminate in this root certificate.</param>
        /// <param name="revocationChecks">Specifies what kind of revocation check should be performed. None by default.</param>
        /// <returns></returns>
        public static bool VerifyCertificate(X509Certificate2 certificateToValidate, X509Certificate2 rootCertificate, X509RevocationMode revocationChecks = X509RevocationMode.NoCheck)
        {
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = revocationChecks;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

            // This part is very important. You're adding your known root here.
            // It doesn't have to be in the computer store at all. Neither certificates do.
            chain.ChainPolicy.ExtraStore.Add(rootCertificate);

            bool isChainValid = chain.Build(certificateToValidate);

            if (!isChainValid)
            {
                string[] errors = chain.ChainStatus
                    .Select(x => String.Format("{0} ({1})", x.StatusInformation.Trim(), x.Status))
                    .ToArray();
                string certificateErrorsString = "Unknown errors.";

                if (errors != null && errors.Length > 0)
                {
                    certificateErrorsString = String.Join(", ", errors);
                }

                throw new Exception("Trust chain did not complete to the known authority anchor. Errors: " + certificateErrorsString);
            }

            // This piece makes sure it actually matches your known root
            if (!chain.ChainElements
                .Cast<X509ChainElement>()
                .Any(x => x.Certificate.Thumbprint == rootCertificate.Thumbprint))
            {
                throw new Exception("Trust chain did not complete to the known authority anchor. Thumbprints did not match.");
            }
            return true;
        }

        #endregion
    }
}