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

public enum pjmedia_event_type {
  PJMEDIA_EVENT_NONE,
  PJMEDIA_EVENT_FMT_CHANGED = ((('H' << 24)|('C' << 16))|('M' << 8))|'F',
  PJMEDIA_EVENT_WND_CLOSING = ((('L' << 24)|('C' << 16))|('N' << 8))|'W',
  PJMEDIA_EVENT_WND_CLOSED = ((('O' << 24)|('C' << 16))|('N' << 8))|'W',
  PJMEDIA_EVENT_WND_RESIZED = ((('Z' << 24)|('R' << 16))|('N' << 8))|'W',
  PJMEDIA_EVENT_MOUSE_BTN_DOWN = ((('N' << 24)|('D' << 16))|('S' << 8))|'M',
  PJMEDIA_EVENT_KEYFRAME_FOUND = ((('F' << 24)|('R' << 16))|('F' << 8))|'I',
  PJMEDIA_EVENT_KEYFRAME_MISSING = ((('M' << 24)|('R' << 16))|('F' << 8))|'I',
  PJMEDIA_EVENT_ORIENT_CHANGED = ((('T' << 24)|('N' << 16))|('R' << 8))|'O',
  PJMEDIA_EVENT_RX_RTCP_FB = ((('B' << 24)|('F' << 16))|('T' << 8))|'R',
  PJMEDIA_EVENT_AUD_DEV_ERROR = ((('R' << 24)|('R' << 16))|('E' << 8))|'A',
  PJMEDIA_EVENT_VID_DEV_ERROR = ((('R' << 24)|('R' << 16))|('E' << 8))|'V',
  PJMEDIA_EVENT_MEDIA_TP_ERR = ((('R' << 24)|('R' << 16))|('E' << 8))|'T',
  PJMEDIA_EVENT_CALLBACK = (((' ' << 24)|(' ' << 16))|('B' << 8))|'C'
}

}
