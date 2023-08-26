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

public class CallVidSetStreamParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal CallVidSetStreamParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(CallVidSetStreamParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(CallVidSetStreamParam obj) {
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

  ~CallVidSetStreamParam() {
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
          pjsua2PINVOKE.delete_CallVidSetStreamParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int medIdx {
    set {
      pjsua2PINVOKE.CallVidSetStreamParam_medIdx_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.CallVidSetStreamParam_medIdx_get(swigCPtr);
      return ret;
    } 
  }

  public pjmedia_dir dir {
    set {
      pjsua2PINVOKE.CallVidSetStreamParam_dir_set(swigCPtr, (int)value);
    } 
    get {
      pjmedia_dir ret = (pjmedia_dir)pjsua2PINVOKE.CallVidSetStreamParam_dir_get(swigCPtr);
      return ret;
    } 
  }

  public int capDev {
    set {
      pjsua2PINVOKE.CallVidSetStreamParam_capDev_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.CallVidSetStreamParam_capDev_get(swigCPtr);
      return ret;
    } 
  }

  public CallVidSetStreamParam() : this(pjsua2PINVOKE.new_CallVidSetStreamParam(), true) {
  }

}

}
