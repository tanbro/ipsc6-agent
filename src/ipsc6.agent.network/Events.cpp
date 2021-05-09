/* clang-format off */
#include "pch.h"
/* clang-format on */

#include "Events.h"

namespace ipsc6 {
namespace agent {
namespace network {

String ^ AgentMessageReceivedEventArgs::ToString() {
    return String::Format(
        "<{0} at 0x{1:x8} CommandType={2}, N1={3}, N2={4}, S=\"{5}\">",
        GetType()->Name, GetHashCode(), CommandType, N1, N2, S);
}

}  // namespace network
}  // namespace agent
}  // namespace ipsc6
