﻿using System.Collections.Generic;
using System.Xml;

namespace FirmaXadesNet.Signature.Parameters
{
    public class SignatureCommitment
    {
        #region Public properties

        public SignatureCommitmentType CommitmentType { get; set; }

        public List<XmlElement> CommitmentTypeQualifiers { get; private set; }

        #endregion

        #region Constructors

        public SignatureCommitment(SignatureCommitmentType commitmentType)
        {
            this.CommitmentType = commitmentType;
            this.CommitmentTypeQualifiers = new List<XmlElement>();
        }
        
        #endregion

        #region Public methods

        public void AddQualifierFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            this.CommitmentTypeQualifiers.Add(doc.DocumentElement);
        }

        #endregion

    }
}
