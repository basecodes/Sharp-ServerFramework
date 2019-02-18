

local Factory = {}

Factory.DictKBVB = FieldType.DictKBVB
Factory.DictKBVP = FieldType.DictKBVP
Factory.DictKPVP = FieldType.DictKPVP
Factory.DictKPVB = FieldType.DictKPVB

function Factory:CreateArray(baseType)
	return {Type = FieldType.ArrayBase,ElementTypeCode = baseType,Value = nil}
end

function Factory:CreateArrayPacket()
	return {Type = FieldType.ArrayPacket,Value = nil}
end

function Factory:CreateDictionary(type,keyType,valueType)
	return {Type = type,KeyType = keyType,ValueType = valueType,Value = nil}
end

function Factory:CreateBase(baseType)
	return {Type = FieldType.BaseType,TypeCode = baseType,Value = nil}
end

function Factory:GetType(baseType)
	if baseType == nil then
		return nil
	end
	return LuaUtils.GetType(baseType)
end


return Factory