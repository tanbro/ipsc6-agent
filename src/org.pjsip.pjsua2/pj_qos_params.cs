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

public class pj_qos_params : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal pj_qos_params(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(pj_qos_params obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~pj_qos_params() {
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
          pjsua2PINVOKE.delete_pj_qos_params(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public byte flags {
    set {
      pjsua2PINVOKE.pj_qos_params_flags_set(swigCPtr, value);
    } 
    get {
      byte ret = pjsua2PINVOKE.pj_qos_params_flags_get(swigCPtr);
      return ret;
    } 
  }

  public byte dscp_val {
    set {
      pjsua2PINVOKE.pj_qos_params_dscp_val_set(swigCPtr, value);
    } 
    get {
      byte ret = pjsua2PINVOKE.pj_qos_params_dscp_val_get(swigCPtr);
      return ret;
    } 
  }

  public byte so_prio {
    set {
      pjsua2PINVOKE.pj_qos_params_so_prio_set(swigCPtr, value);
    } 
    get {
      byte ret = pjsua2PINVOKE.pj_qos_params_so_prio_get(swigCPtr);
      return ret;
    } 
  }

  public pj_qos_wmm_prio wmm_prio {
    set {
      pjsua2PINVOKE.pj_qos_params_wmm_prio_set(swigCPtr, (int)value);
    } 
    get {
      pj_qos_wmm_prio ret = (pj_qos_wmm_prio)pjsua2PINVOKE.pj_qos_params_wmm_prio_get(swigCPtr);
      return ret;
    } 
  }

  public pj_qos_params() : this(pjsua2PINVOKE.new_pj_qos_params(), true) {
  }

}

}
