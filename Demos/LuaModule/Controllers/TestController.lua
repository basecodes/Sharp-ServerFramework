local Classes = require "Classes"
local Controller = require "Controller"
local Factory = require "Factory"
local BaseType = require "BaseType"

local TestController = Classes.class(Controller)

function TestController:init()
    self.super:init(self)

	self:Register("User-5F674579-4D3F-42DE-A72C-A8B46AE94908", self.Test1)
	self:Register("User-1ECE00D8-614A-481F-861E-D20EEA55247C", self.Test2)
	self:Register("User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB", self.Test3)
	self:Register("User-4844F488-2169-4AAE-A93B-56E45E10495B", self.Test4)
end

function TestController:Test1(num,str,peer,callback)
    print(num.Value)
	print(str.Value)

	self:Invoke("User-9CEF8CD0-8720-4C34-9341-545AF7693AB2",peer,nil,num,str)
	return true
end

function TestController:Test2(num,str,array,peer,callback)
    print(num.Value)
	print(str.Value)

	for k,v in pairs(array.Value) do
		print(k .. " " .. tostring(v))
	end

	self:Invoke("User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C",peer,nil,num,str,array)
	return true
end

function TestController:Test3(num,str,array,packets,peer,callback)
    print(num.Value)
	print(str.Value)

	for item in pairs(array.Value) do
		print(item)
	end

	for k,v in pairs(packets.Value) do
		print(k .. " " .. tostring(v))
	end

	self:Invoke("User-444E0735-DA0B-4A29-9746-E7FEFE7E2293",peer,nil,num,str,array,packets)
	return true
end

function TestController:Test4(str,packets,peer,callback)
	print(str.Value)

	for k,v in pairs(packets.Value) do
		print(k .. " " .. tostring(v))
	end

	self:Invoke("User-C00E44A2-09E0-4C94-81DC-9622AA38EFB4",peer,nil,str,packets)
	return true
end

return TestController