local classes = require "Classes"

local ITestRequest = classes.class()

function ITestRequest:init(child)
	self.super:init(child)
end

-- User-9CEF8CD0-8720-4C34-9341-545AF7693AB2
function ITestRequest:Test1(str)
	return true
end

-- User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C
function ITestRequest:Test2(str,packet)
	return true
end

return ITestRequest