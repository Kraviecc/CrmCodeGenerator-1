﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using CrmCodeGenerator.VSPackage.Helpers;

namespace CrmCodeGenerator.VSPackage.Model
{
    [Serializable]
    public class MappingRelationshipN1
    {
        public CrmRelationshipAttribute Attribute { get; set; }

        public string DisplayName
        {
            get;
            set;
        }

        public string ForeignKey
        {
            get;
            set;
        }

        public string PrivateName
        {
            get;
            set;
        }

        public string Type
        {
            get
            {
                return Naming.GetProperVariableName(Attribute.ToEntity);
            }
        }

        public static MappingRelationshipN1 Parse(OneToManyRelationshipMetadata rel, MappingField[] properties)
        {
            var propertyName =
                properties.First(p => p.Attribute.LogicalName.ToLower() == rel.ReferencingAttribute.ToLower()).DisplayName;

            return new MappingRelationshipN1
            {
                Attribute = new CrmRelationshipAttribute
                {
                    ToEntity = rel.ReferencedEntity,
                    ToKey = rel.ReferencedAttribute,
                    FromEntity = rel.ReferencingEntity,
                    FromKey = rel.ReferencingAttribute,
                    IntersectingEntity = ""
                },
                DisplayName = Naming.GetProperVariableName(rel.SchemaName) +"_N1",
                PrivateName = "_n1"+ Naming.GetEntityPropertyPrivateName(rel.SchemaName),
                ForeignKey = propertyName
            };
        }
    }
}