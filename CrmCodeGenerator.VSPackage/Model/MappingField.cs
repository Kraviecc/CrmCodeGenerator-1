﻿using CrmCodeGenerator.VSPackage.Helpers;
using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace CrmCodeGenerator.VSPackage.Model
{
    [Serializable]
    public class MappingField
    {
        public MappingField()
        {
            IsValidForUpdate = false;
            IsValidForCreate = false;
            IsDeprecated = false;
            Description = "";
        }

        public CrmPropertyAttribute Attribute { get; set; }
        public AttributeMetadata AttributeMetadata { get; set; }
        public string AttributeOf { get; set; }
        public string AttributeTypeName { get; private set; }
        public string DeprecatedVersion { get; set; }
        public string Description { get; set; }
        public string DescriptionXmlSafe
        {
            get
            {
                return Naming.XmlEscape(Description);
            }
        }

        public string DisplayName { get; set; }
        public MappingEntity Entity { get; set; }
        public MappingEnum EnumData { get; set; }
        public AttributeTypeCode FieldType { get; set; }
        public string FieldTypeString { get; set; }
        public string GetMethod { get; set; }
        public string HybridName { get; set; }
        public bool IsActivityParty { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsOptionSet { get; private set; }
        public bool IsRequired { get; set; }
        public bool IsStateCode { get; set; }
        public bool IsTwoOption { get; private set; }
        public bool IsValidForCreate { get; set; }
        public bool IsValidForRead { get; set; }
        public bool IsValidForUpdate { get; set; }
        public string Label { get; set; }
        public string LogicalName { get; set; }
        public string LookupSingleType { get; set; }
        public decimal? Max { get; set; }
        public int? MaxLength { get; set; }
        public decimal? Min { get; set; }
        public string PrivatePropertyName { get; set; }
        public string SetMethodCall
        {
            get
            {
                var methodName = "";

                switch (FieldType)
                {
                    case AttributeTypeCode.Picklist:
                        methodName = "SetPicklist"; break;
                    case AttributeTypeCode.BigInt:
                    case AttributeTypeCode.Integer:
                        methodName = "SetValue<int?>"; break;
                    case AttributeTypeCode.Boolean:
                        methodName = "SetValue<bool?>"; break;
                    case AttributeTypeCode.DateTime:
                        methodName = "SetValue<DateTime?>"; break;
                    case AttributeTypeCode.Decimal:
                        methodName = "SetValue<decimal?>"; break;
                    case AttributeTypeCode.Money:
                        methodName = "SetMoney"; break;
                    case AttributeTypeCode.Memo:
                    case AttributeTypeCode.String:
                        methodName = "SetValue<string>"; break;
                    case AttributeTypeCode.Double:
                        methodName = "SetValue<double?>"; break;
                    case AttributeTypeCode.Uniqueidentifier:
                        methodName = "SetValue<Guid?>"; break;
                    case AttributeTypeCode.Lookup:
                        methodName = "SetLookup"; break;
                    //methodName = "SetLookup"; break;
                    case AttributeTypeCode.Virtual:
                        if (AttributeTypeName == "MultiSelectPicklistType")
                        {
                            return "SetValue<OptionSetValueCollection>";
                        }
                        methodName = "SetValue<string>"; break;
                    case AttributeTypeCode.Customer:
                        methodName = "SetCustomer"; break;
                    case AttributeTypeCode.Status:
                        methodName = ""; break;
                    case AttributeTypeCode.EntityName:
                        methodName = "SetEntityNameReference"; break;
                    case AttributeTypeCode.State:
                    case AttributeTypeCode.Owner:
                    default:
                        return "";
                }

                if (methodName == "" || !IsValidForUpdate)
                    return "";

                switch (FieldType)
                {
                    case AttributeTypeCode.Picklist:
                        return $"{methodName}(\"{this.Attribute.LogicalName}\", (int?)value);";
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Customer:
                        return string.IsNullOrEmpty(LookupSingleType)
                            ? $"{methodName}(\"{Attribute.LogicalName}\", {this.DisplayName}Type, value);"
                            : $"{methodName}(\"{Attribute.LogicalName}\", \"{this.LookupSingleType}\", value);";
                }

                return $"{methodName}(\"{this.Attribute.LogicalName}\", value);";
            }
        }

        public string StateName { get; set; }
        public string TargetType
        {
            get
            {
                if (IsPrimaryKey)
                    return "Guid";

                switch (FieldType)
                {
                    case AttributeTypeCode.Picklist:
                        return $"Enums.{EnumData.DisplayName}?";

                    case AttributeTypeCode.BigInt:
                    case AttributeTypeCode.Integer:
                        return "int?";

                    case AttributeTypeCode.Boolean:
                        return "bool?";

                    case AttributeTypeCode.DateTime:
                        return "DateTime?";

                    case AttributeTypeCode.Decimal:
                    case AttributeTypeCode.Money:
                        return "decimal?";

                    case AttributeTypeCode.Double:
                        return "double?";

                    case AttributeTypeCode.Uniqueidentifier:
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Owner:
                    case AttributeTypeCode.Customer:
                        return "Guid?";

                    case AttributeTypeCode.State:
                    case AttributeTypeCode.Status:
                        return "int";

                    case AttributeTypeCode.Memo:
                    case AttributeTypeCode.Virtual:
                    case AttributeTypeCode.EntityName:
                    case AttributeTypeCode.String:
                        if (AttributeTypeName == "MultiSelectPicklistType")
                        {
                            return "OptionSetValueCollection";
                        }
                        return "string";

                    default:
                        return "object";
                }
            }
        }

        public string TargetTypeForCrmSvcUtil { get; set; }
        private bool IsPrimaryKey { get; set; }
        public static MappingField Parse(AttributeMetadata attribute, MappingEntity entity)
        {
            var result = new MappingField();
            result.Entity = entity;
            result.AttributeOf = attribute.AttributeOf;
            if (attribute.IsValidForCreate != null) result.IsValidForCreate = (bool)attribute.IsValidForCreate;
            if (attribute.IsValidForRead != null) result.IsValidForRead = (bool)attribute.IsValidForRead;
            if (attribute.IsValidForUpdate != null) result.IsValidForUpdate = (bool)attribute.IsValidForUpdate;
            result.IsActivityParty = attribute.AttributeType == AttributeTypeCode.PartyList;
            result.IsStateCode = attribute.AttributeType == AttributeTypeCode.State;
            result.IsOptionSet = attribute.AttributeType == AttributeTypeCode.Picklist;
            result.IsTwoOption = attribute.AttributeType == AttributeTypeCode.Boolean;
            result.DeprecatedVersion = attribute.DeprecatedVersion;
            result.IsDeprecated = !string.IsNullOrWhiteSpace(attribute.DeprecatedVersion);

            switch (attribute)
            {
                case PicklistAttributeMetadata _:
                    result.EnumData =
                        MappingEnum.Parse(attribute as PicklistAttributeMetadata);
                    break;

                case MultiSelectPicklistAttributeMetadata _:
                    result.EnumData =
                        MappingEnum.Parse(attribute as MultiSelectPicklistAttributeMetadata);
                    break;
            }

            var lookup = attribute as LookupAttributeMetadata;

            if (lookup?.Targets.Length == 1)
                result.LookupSingleType = lookup.Targets[0];

            ParseMinMaxValues(attribute, result);

            if (attribute.AttributeType != null)
                result.FieldType = attribute.AttributeType.Value;
            if (attribute.AttributeTypeName != null)
            {
                result.AttributeTypeName = attribute.AttributeTypeName.Value;
            }

            result.IsPrimaryKey = attribute.IsPrimaryId == true;

            result.LogicalName = attribute.LogicalName;
            result.DisplayName = Naming.GetProperVariableName(attribute);
            result.PrivatePropertyName = Naming.GetEntityPropertyPrivateName(attribute.SchemaName);
            result.HybridName = Naming.GetProperHybridFieldName(result.DisplayName, result.Attribute);

            if (attribute.Description?.UserLocalizedLabel != null)
                result.Description = attribute.Description.UserLocalizedLabel.Label;

            if (attribute.DisplayName?.UserLocalizedLabel != null)
                result.Label = attribute.DisplayName.UserLocalizedLabel.Label;

            result.IsRequired = attribute.RequiredLevel != null && attribute.RequiredLevel.Value == AttributeRequiredLevel.ApplicationRequired;

            result.Attribute =
                new CrmPropertyAttribute
                {
                    LogicalName = attribute.LogicalName,
                    IsLookup = attribute.AttributeType == AttributeTypeCode.Lookup || attribute.AttributeType == AttributeTypeCode.Customer
                };
            result.TargetTypeForCrmSvcUtil = GetTargetType(result);
            result.FieldTypeString = result.TargetTypeForCrmSvcUtil;

            return result;
        }

        private static string GetTargetType(MappingField field)
        {
            if (field.IsPrimaryKey)
                return "Guid?";

            switch (field.FieldType)
            {
                case AttributeTypeCode.Picklist:
                    return "OptionSetValue";

                case AttributeTypeCode.BigInt:
                    return "long?";

                case AttributeTypeCode.Integer:
                    return "int?";

                case AttributeTypeCode.Boolean:
                    return "bool?";

                case AttributeTypeCode.DateTime:
                    return "DateTime?";

                case AttributeTypeCode.Decimal:
                    return "decimal?";

                case AttributeTypeCode.Money:
                    return "Money";

                case AttributeTypeCode.Double:
                    return "double?";

                case AttributeTypeCode.Uniqueidentifier:
                    return "Guid?";

                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                    return "EntityReference";

                case AttributeTypeCode.State:
                    return field.Entity.StateName + "?";

                case AttributeTypeCode.Status:
                    return "OptionSetValue";

                case AttributeTypeCode.Memo:
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.String:
                    if (field.AttributeTypeName == "MultiSelectPicklistType")
                    {
                        return ".OptionSetValueCollection";
                    }
                    return "string";

                case AttributeTypeCode.PartyList:
                    return "IEnumerable<ActivityParty>";

                case AttributeTypeCode.ManagedProperty:
                    return "BooleanManagedProperty";

                default:
                    return "object";
            }
        }

        private static void ParseMinMaxValues(AttributeMetadata attribute, MappingField result)
        {
            switch (attribute)
            {
                case StringAttributeMetadata _:
                    result.MaxLength = ((StringAttributeMetadata)attribute).MaxLength ?? -1;
                    break;

                case MemoAttributeMetadata _:
                    result.MaxLength = ((MemoAttributeMetadata)attribute).MaxLength ?? -1;
                    break;

                case IntegerAttributeMetadata _:
                    {
                        if (attribute is IntegerAttributeMetadata attr)
                        {
                            result.Min = attr.MinValue ?? -1;
                            result.Max = attr.MaxValue ?? -1;
                        }
                        break;
                    }
                case DecimalAttributeMetadata _:
                    {
                        if (attribute is DecimalAttributeMetadata attr)
                        {
                            result.Min = attr.MinValue ?? -1;
                            result.Max = attr.MaxValue ?? -1;
                        }
                        break;
                    }
                case MoneyAttributeMetadata _:
                    {
                        if (attribute is MoneyAttributeMetadata attr)
                        {
                            result.Min = attr.MinValue != null ? (decimal)attr.MinValue.Value : -1;
                            result.Max = attr.MaxValue != null ? (decimal)attr.MaxValue.Value : -1;
                        }
                        break;
                    }
                case DoubleAttributeMetadata _:
                    {
                        if (attribute is DoubleAttributeMetadata attr)
                        {
                            result.Min = attr.MinValue != null ? (decimal)attr.MinValue.Value : -1;
                            result.Max = attr.MaxValue != null ? (decimal)attr.MaxValue.Value : -1;
                        }
                        break;
                    }
            }
        }
    }
}