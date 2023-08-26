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

public class VideoPreviewOpParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal VideoPreviewOpParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(VideoPreviewOpParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(VideoPreviewOpParam obj) {
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

  ~VideoPreviewOpParam() {
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
          pjsua2PINVOKE.delete_VideoPreviewOpParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int rendId {
    set {
      pjsua2PINVOKE.VideoPreviewOpParam_rendId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.VideoPreviewOpParam_rendId_get(swigCPtr);
      return ret;
    } 
  }

  public bool show {
    set {
      pjsua2PINVOKE.VideoPreviewOpParam_show_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.VideoPreviewOpParam_show_get(swigCPtr);
      return ret;
    } 
  }

  public uint windowFlags {
    set {
      pjsua2PINVOKE.VideoPreviewOpParam_windowFlags_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.VideoPreviewOpParam_windowFlags_get(swigCPtr);
      return ret;
    } 
  }

  public MediaFormat format {
    set {
      pjsua2PINVOKE.VideoPreviewOpParam_format_set(swigCPtr, MediaFormat.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VideoPreviewOpParam_format_get(swigCPtr);
      MediaFormat ret = (cPtr == global::System.IntPtr.Zero) ? null : new MediaFormat(cPtr, false);
      return ret;
    } 
  }

  public VideoWindowHandle window {
    set {
      pjsua2PINVOKE.VideoPreviewOpParam_window_set(swigCPtr, VideoWindowHandle.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VideoPreviewOpParam_window_get(swigCPtr);
      VideoWindowHandle ret = (cPtr == global::System.IntPtr.Zero) ? null : new VideoWindowHandle(cPtr, false);
      return ret;
    } 
  }

  public VideoPreviewOpParam() : this(pjsua2PINVOKE.new_VideoPreviewOpParam(), true) {
  }

}

}
