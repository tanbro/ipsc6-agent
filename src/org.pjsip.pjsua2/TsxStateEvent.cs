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

public class TsxStateEvent : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal TsxStateEvent(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(TsxStateEvent obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~TsxStateEvent() {
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
          pjsua2PINVOKE.delete_TsxStateEvent(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public TsxStateEventSrc src {
    set {
      pjsua2PINVOKE.TsxStateEvent_src_set(swigCPtr, TsxStateEventSrc.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TsxStateEvent_src_get(swigCPtr);
      TsxStateEventSrc ret = (cPtr == global::System.IntPtr.Zero) ? null : new TsxStateEventSrc(cPtr, false);
      return ret;
    } 
  }

  public SipTransaction tsx {
    set {
      pjsua2PINVOKE.TsxStateEvent_tsx_set(swigCPtr, SipTransaction.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TsxStateEvent_tsx_get(swigCPtr);
      SipTransaction ret = (cPtr == global::System.IntPtr.Zero) ? null : new SipTransaction(cPtr, false);
      return ret;
    } 
  }

  public pjsip_tsx_state_e prevState {
    set {
      pjsua2PINVOKE.TsxStateEvent_prevState_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_tsx_state_e ret = (pjsip_tsx_state_e)pjsua2PINVOKE.TsxStateEvent_prevState_get(swigCPtr);
      return ret;
    } 
  }

  public pjsip_event_id_e type {
    set {
      pjsua2PINVOKE.TsxStateEvent_type_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_event_id_e ret = (pjsip_event_id_e)pjsua2PINVOKE.TsxStateEvent_type_get(swigCPtr);
      return ret;
    } 
  }

  public TsxStateEvent() : this(pjsua2PINVOKE.new_TsxStateEvent(), true) {
  }

}

}
