#pragma once

#include "framework.h"

namespace ipsc6 {
namespace agent {
namespace network {

ref class Connection {
   public:
    Connection(System::String ^ host, System::UInt16 ^ port);
    ~Connection();

   private:
    System::String ^ _host;

   public:
    static void Initial();
    static void Release();

    property System::String ^ host { System::String ^ get() { return _host; } }
};

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
