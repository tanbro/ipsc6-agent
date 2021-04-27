//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace org.pjsip.pjsua2 {

public class AccountIpChangeConfig : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal AccountIpChangeConfig(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AccountIpChangeConfig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~AccountIpChangeConfig() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          pjsua2PINVOKE.delete_AccountIpChangeConfig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public bool shutdownTp {
    set {
      pjsua2PINVOKE.AccountIpChangeConfig_shutdownTp_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountIpChangeConfig_shutdownTp_get(swigCPtr);
      return ret;
    } 
  }

  public bool hangupCalls {
    set {
      pjsua2PINVOKE.AccountIpChangeConfig_hangupCalls_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountIpChangeConfig_hangupCalls_get(swigCPtr);
      return ret;
    } 
  }

  public uint reinviteFlags {
    set {
      pjsua2PINVOKE.AccountIpChangeConfig_reinviteFlags_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.AccountIpChangeConfig_reinviteFlags_get(swigCPtr);
      return ret;
    } 
  }

  public virtual void readObject(ContainerNode node) {
    pjsua2PINVOKE.AccountIpChangeConfig_readObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void writeObject(ContainerNode node) {
    pjsua2PINVOKE.AccountIpChangeConfig_writeObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AccountIpChangeConfig() : this(pjsua2PINVOKE.new_AccountIpChangeConfig(), true) {
  }

}

}
