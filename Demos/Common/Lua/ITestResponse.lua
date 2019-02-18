local classes = require "Classes"

local ITestResponse = classes.class()

function ITestResponse:init(child)
	self.super:init(child)
end

-- User-5F674579-4D3F-42DE-A72C-A8B46AE94908
function ITestResponse:Test1(num,str,peer,callback)
	return true
end

-- User-1ECE00D8-614A-481F-861E-D20EEA55247C
function ITestResponse:Test2(num,str,array,peer,callback)
	return true
end

-- User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB
function ITestResponse:Test3(num,str,array,packets,peer,callback)
	return true
end

-- User-4844F488-2169-4AAE-A93B-56E45E10495B
function ITestResponse:Test4(str,packets,peer,callback)
	return true
end

return ITestResponse