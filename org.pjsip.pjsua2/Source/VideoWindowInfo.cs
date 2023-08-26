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

public class VideoWindowInfo : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal VideoWindowInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(VideoWindowInfo obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(VideoWindowInfo obj) {
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

  ~VideoWindowInfo() {
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
          pjsua2PINVOKE.delete_VideoWindowInfo(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public bool isNative {
    set {
      pjsua2PINVOKE.VideoWindowInfo_isNative_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.VideoWindowInfo_isNative_get(swigCPtr);
      return ret;
    } 
  }

  public VideoWindowHandle winHandle {
    set {
      pjsua2PINVOKE.VideoWindowInfo_winHandle_set(swigCPtr, VideoWindowHandle.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VideoWindowInfo_winHandle_get(swigCPtr);
      VideoWindowHandle ret = (cPtr == global::System.IntPtr.Zero) ? null : new VideoWindowHandle(cPtr, false);
      return ret;
    } 
  }

  public int renderDeviceId {
    set {
      pjsua2PINVOKE.VideoWindowInfo_renderDeviceId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.VideoWindowInfo_renderDeviceId_get(swigCPtr);
      return ret;
    } 
  }

  public bool show {
    set {
      pjsua2PINVOKE.VideoWindowInfo_show_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.VideoWindowInfo_show_get(swigCPtr);
      return ret;
    } 
  }

  public MediaCoordinate pos {
    set {
      pjsua2PINVOKE.VideoWindowInfo_pos_set(swigCPtr, MediaCoordinate.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VideoWindowInfo_pos_get(swigCPtr);
      MediaCoordinate ret = (cPtr == global::System.IntPtr.Zero) ? null : new MediaCoordinate(cPtr, false);
      return ret;
    } 
  }

  public MediaSize size {
    set {
      pjsua2PINVOKE.VideoWindowInfo_size_set(swigCPtr, MediaSize.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VideoWindowInfo_size_get(swigCPtr);
      MediaSize ret = (cPtr == global::System.IntPtr.Zero) ? null : new MediaSize(cPtr, false);
      return ret;
    } 
  }

  public VideoWindowInfo() : this(pjsua2PINVOKE.new_VideoWindowInfo(), true) {
  }

}

}
