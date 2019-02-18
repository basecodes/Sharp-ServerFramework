local Classes = require "Classes"
local Controller = require "Controller"

local TestController = Classes.class(Controller)

function TestController:init()
    self.super:init(self)

	self:Register("User-5F674579-4D3F-42DE-A72C-A8B46AE94908", self.Test1)
	self:Register("User-1ECE00D8-614A-481F-861E-D20EEA55247C", self.Test2)
	self:Register("User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB", self.Test3)
end

function TestController:Test1(num,str,peer,callback)
    print(num)
	print(str)

	self:Invoke("User-9CEF8CD0-8720-4C34-9341-545AF7693AB2",peer,nil,num .. str)
	return true
end

function TestController:Test2(num,str,array,peer,callback)
    print(num)
	print(str)

	for k,v in pairs(array) do
		print(k .. " " .. tostring(v))
	end

	return true
end

function TestController:Test3(num,str,array,packets,peer,callback)
    print(num)
	print(str)

	for item in pairs(array) do
		print(item)
	end

	for k,v in pairs(packets) do
		print(k .. " " .. tostring(v))
	end

	self:Invoke("User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C",peer,nil,num .. str,packets[1])
	return true
end

return TestController