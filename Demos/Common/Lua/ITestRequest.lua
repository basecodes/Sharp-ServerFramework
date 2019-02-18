local classes = require "Classes"

local ITestRequest = classes.class()

function ITestRequest:init(child)
	self.super:init(child)
end

-- User-9CEF8CD0-8720-4C34-9341-545AF7693AB2
function ITestRequest:Test1(int_num,str)
	return true
end

-- User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C
function ITestRequest:Test2(int_num,str,array)
	return true
end

-- User-444E0735-DA0B-4A29-9746-E7FEFE7E2293
function ITestRequest:Test3(int_num,str,array,testPackets)
	return true
end

-- User-C00E44A2-09E0-4C94-81DC-9622AA38EFB4
function ITestRequest:Test4(str,dict)
	return true
end

return ITestRequest