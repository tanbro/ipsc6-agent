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

public class OnCreateMediaTransportSrtpParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal OnCreateMediaTransportSrtpParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(OnCreateMediaTransportSrtpParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(OnCreateMediaTransportSrtpParam obj) {
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

  ~OnCreateMediaTransportSrtpParam() {
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
          pjsua2PINVOKE.delete_OnCreateMediaTransportSrtpParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public uint mediaIdx {
    set {
      pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_mediaIdx_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_mediaIdx_get(swigCPtr);
      return ret;
    } 
  }

  public pjmedia_srtp_use srtpUse {
    set {
      pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_srtpUse_set(swigCPtr, (int)value);
    } 
    get {
      pjmedia_srtp_use ret = (pjmedia_srtp_use)pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_srtpUse_get(swigCPtr);
      return ret;
    } 
  }

  public SrtpCryptoVector cryptos {
    set {
      pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_cryptos_set(swigCPtr, SrtpCryptoVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.OnCreateMediaTransportSrtpParam_cryptos_get(swigCPtr);
      SrtpCryptoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new SrtpCryptoVector(cPtr, false);
      return ret;
    } 
  }

  public OnCreateMediaTransportSrtpParam() : this(pjsua2PINVOKE.new_OnCreateMediaTransportSrtpParam(), true) {
  }

}

}
