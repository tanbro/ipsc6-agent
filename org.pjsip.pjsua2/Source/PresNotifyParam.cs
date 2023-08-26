//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.1.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace org.pjsip.pjsua2 {

public class PresNotifyParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PresNotifyParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PresNotifyParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(PresNotifyParam obj) {
    if (obj != null) {
      if (!obj.swigCMemOwn)
        throw new global::System.ApplicationException("Cannot release ownership as memory is not owned");
      global::System.Runtime.InteropServices.HandleRef ptr = obj.swigCPtr;
      obj.swigCMemOwn = false;
      obj.Dispose();
      return ptr;
    } else {
      return new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
    }
  }

  ~PresNotifyParam() {
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
          pjsua2PINVOKE.delete_PresNotifyParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public SWIGTYPE_p_void srvPres {
    set {
      pjsua2PINVOKE.PresNotifyParam_srvPres_set(swigCPtr, SWIGTYPE_p_void.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.PresNotifyParam_srvPres_get(swigCPtr);
      SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
      return ret;
    } 
  }

  public pjsip_evsub_state state {
    set {
      pjsua2PINVOKE.PresNotifyParam_state_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_evsub_state ret = (pjsip_evsub_state)pjsua2PINVOKE.PresNotifyParam_state_get(swigCPtr);
      return ret;
    } 
  }

  public string stateStr {
    set {
      pjsua2PINVOKE.PresNotifyParam_stateStr_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.PresNotifyParam_stateStr_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string reason {
    set {
      pjsua2PINVOKE.PresNotifyParam_reason_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.PresNotifyParam_reason_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public bool withBody {
    set {
      pjsua2PINVOKE.PresNotifyParam_withBody_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.PresNotifyParam_withBody_get(swigCPtr);
      return ret;
    } 
  }

  public SipTxOption txOption {
    set {
      pjsua2PINVOKE.PresNotifyParam_txOption_set(swigCPtr, SipTxOption.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.PresNotifyParam_txOption_get(swigCPtr);
      SipTxOption ret = (cPtr == global::System.IntPtr.Zero) ? null : new SipTxOption(cPtr, false);
      return ret;
    } 
  }

  public PresNotifyParam() : this(pjsua2PINVOKE.new_PresNotifyParam(), true) {
  }

}

}
