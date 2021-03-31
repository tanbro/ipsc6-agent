#pragma once

using namespace System;

namespace ipsc6 {
	namespace agent {
		namespace network {

			ref class Connection
			{
			public:
				Connection(::String^ host, ::UInt16^ port);
				static void Initial();
				static void Release();
			};


		}
	}
}
