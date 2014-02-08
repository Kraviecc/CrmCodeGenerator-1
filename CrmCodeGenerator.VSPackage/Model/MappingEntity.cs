﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using CrmCodeGenerator.VSPackage.Helpers;

namespace CrmCodeGenerator.VSPackage.Model
{
    [Serializable]
    public class MappingEntity
    {
        public CrmEntityAttribute Attribute { get; set; }

        public MappingField[] Fields { get; set; }
        public MappingEnum[] Enums { get; set; }
        public MappingRelationship1N[] RelationshipsOneToMany { get; set; }
        public MappingRelationshipN1[] RelationshipsManyToOne { get; set; }

        public string LogicalName
        {
            get
            {
                return Attribute.LogicalName;
            }
        }

        public string DisplayName
        {
            get;
            set;
        }
        public string HybridName { get; set; }

        public MappingField PrimaryKey { get; set; }
        public string PrimaryKeyProperty
        {
            get;
            set;
        }

        public string Plural
        {
            get
            {
                return Naming.GetPluralName(DisplayName);
            }
        }

        public static MappingEntity Parse(EntityMetadata entityMetadata)
        {
            var entity = new MappingEntity();

            entity.Attribute = new CrmEntityAttribute();

            entity.Attribute.LogicalName = entityMetadata.LogicalName;
            entity.Attribute.PrimaryKey = entityMetadata.PrimaryIdAttribute;

            // entity.DisplayName = Helper.GetProperVariableName(entityMetadata.SchemaName);
            entity.DisplayName = Naming.GetProperEntityName(entityMetadata.SchemaName);
            entity.HybridName = Naming.GetProperHybridName(entityMetadata.SchemaName, entityMetadata.LogicalName);

            var fields = entityMetadata.Attributes
                .Where(a => !(a.LogicalName.EndsWith("_base") && a.AttributeType == AttributeTypeCode.Money) && a.AttributeType != AttributeTypeCode.EntityName)
                .Select(a => MappingField.Parse(a)).ToList();

            fields.ForEach(f =>
                    {
                        if (f.DisplayName == entity.DisplayName)
                            f.DisplayName += "1";
                        f.HybridName = Naming.GetProperHybridFieldName(f.DisplayName, f.Attribute);
                    }
                );

            var fieldsIterator = fields.Where(e => e.Attribute.IsLookup).ToArray();

            foreach (var lookup in fieldsIterator)
            {
                var nameField = new MappingField
                {
                    Attribute = new CrmPropertyAttribute
                    {
                        IsLookup = false,
                        LogicalName = lookup.Attribute.LogicalName + "Name",
                        IsEntityReferenceHelper = true
                    },
                    DisplayName = lookup.DisplayName + "Name",
                    HybridName = lookup.HybridName  + "Name",
                    FieldType = AttributeTypeCode.EntityName,
                    IsUpdatable = false,
                    GetMethod = "",
                    PrivatePropertyName = lookup.PrivatePropertyName + "Name"
                };

                if (fields.Count(f => f.DisplayName == nameField.DisplayName) == 0)
                    fields.Add(nameField);

                if (!string.IsNullOrEmpty(lookup.LookupSingleType))
                    continue;

                var typeField = new MappingField
                {
                    Attribute = new CrmPropertyAttribute
                    {
                        IsLookup = false,
                        LogicalName = lookup.Attribute.LogicalName + "Type",
                        IsEntityReferenceHelper = true
                    },
                    DisplayName = lookup.DisplayName + "Type",
                    HybridName = lookup.HybridName + "Type",
                    FieldType = AttributeTypeCode.EntityName,
                    IsUpdatable = false,
                    GetMethod = "",
                    PrivatePropertyName = lookup.PrivatePropertyName + "Type"
                };

                if (fields.Count(f => f.DisplayName == typeField.DisplayName) == 0)
                    fields.Add(typeField);

            }

            entity.Fields = fields.ToArray();

            entity.Enums = entityMetadata.Attributes
                .Where(a => a is PicklistAttributeMetadata || a is StateAttributeMetadata || a is StatusAttributeMetadata)
                .Select(a => MappingEnum.Parse(a as EnumAttributeMetadata)).ToArray();
                //.Select(a => MapperEnum.Parse(a as PicklistAttributeMetadata)).ToArray();

            entity.PrimaryKey = entity.Fields.First(f => f.Attribute.LogicalName == entity.Attribute.PrimaryKey);
            entity.PrimaryKeyProperty = entity.PrimaryKey.DisplayName;

            entity.RelationshipsOneToMany = entityMetadata.OneToManyRelationships.Select(r =>
                MappingRelationship1N.Parse(r, entity.Fields)).ToArray();

            entity.RelationshipsManyToOne = entityMetadata.ManyToOneRelationships.Select(r =>
                MappingRelationshipN1.Parse(r, entity.Fields)).ToArray();

            entity.RelationshipsManyToMany = entityMetadata.ManyToManyRelationships.Select(r =>
                MappingRelationshipMN.Parse(r)).ToArray();

            return entity;
        }

        public MappingRelationshipN1[] RelationshipsManyToMany { get; set; }
    }
}