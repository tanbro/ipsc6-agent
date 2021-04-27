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

public class RtcpStreamStat : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RtcpStreamStat(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(RtcpStreamStat obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~RtcpStreamStat() {
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
          pjsua2PINVOKE.delete_RtcpStreamStat(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public TimeVal update {
    set {
      pjsua2PINVOKE.RtcpStreamStat_update_set(swigCPtr, TimeVal.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.RtcpStreamStat_update_get(swigCPtr);
      TimeVal ret = (cPtr == global::System.IntPtr.Zero) ? null : new TimeVal(cPtr, false);
      return ret;
    } 
  }

  public uint updateCount {
    set {
      pjsua2PINVOKE.RtcpStreamStat_updateCount_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_updateCount_get(swigCPtr);
      return ret;
    } 
  }

  public uint pkt {
    set {
      pjsua2PINVOKE.RtcpStreamStat_pkt_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_pkt_get(swigCPtr);
      return ret;
    } 
  }

  public uint bytes {
    set {
      pjsua2PINVOKE.RtcpStreamStat_bytes_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_bytes_get(swigCPtr);
      return ret;
    } 
  }

  public uint discard {
    set {
      pjsua2PINVOKE.RtcpStreamStat_discard_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_discard_get(swigCPtr);
      return ret;
    } 
  }

  public uint loss {
    set {
      pjsua2PINVOKE.RtcpStreamStat_loss_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_loss_get(swigCPtr);
      return ret;
    } 
  }

  public uint reorder {
    set {
      pjsua2PINVOKE.RtcpStreamStat_reorder_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_reorder_get(swigCPtr);
      return ret;
    } 
  }

  public uint dup {
    set {
      pjsua2PINVOKE.RtcpStreamStat_dup_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.RtcpStreamStat_dup_get(swigCPtr);
      return ret;
    } 
  }

  public MathStat lossPeriodUsec {
    set {
      pjsua2PINVOKE.RtcpStreamStat_lossPeriodUsec_set(swigCPtr, MathStat.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.RtcpStreamStat_lossPeriodUsec_get(swigCPtr);
      MathStat ret = (cPtr == global::System.IntPtr.Zero) ? null : new MathStat(cPtr, false);
      return ret;
    } 
  }

  public LossType lossType {
    set {
      pjsua2PINVOKE.RtcpStreamStat_lossType_set(swigCPtr, LossType.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.RtcpStreamStat_lossType_get(swigCPtr);
      LossType ret = (cPtr == global::System.IntPtr.Zero) ? null : new LossType(cPtr, false);
      return ret;
    } 
  }

  public MathStat jitterUsec {
    set {
      pjsua2PINVOKE.RtcpStreamStat_jitterUsec_set(swigCPtr, MathStat.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.RtcpStreamStat_jitterUsec_get(swigCPtr);
      MathStat ret = (cPtr == global::System.IntPtr.Zero) ? null : new MathStat(cPtr, false);
      return ret;
    } 
  }

  public RtcpStreamStat() : this(pjsua2PINVOKE.new_RtcpStreamStat(), true) {
  }

}

}
