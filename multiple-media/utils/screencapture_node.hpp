#include "napi.h"
#include "screencapture.hpp"

Napi::Value SevlabNAddCVMonitorBits(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  if (info.Length() < 2 || !info[0].IsNumber()) {
    throw Napi::TypeError::New(env, "Expected one argument: [Number],[Buffer]");
  }
  int index = info[0].ToNumber().Int32Value();
  Napi::Buffer<uint8_t> buffer = info[1].As<Napi::Buffer<uint8_t>>();
  PBITS res = (PBITS)(buffer.Data());
  bool ret = SevlabMonitorBitsAdd(index, res);
  return Napi::Boolean::New(env, ret);
}
Napi::Value SevlabNClearCVMonitorBits(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  if (info.Length() < 1 || !info[0].IsNumber()) {
    throw Napi::TypeError::New(env, "Expected one argument: [Number]");
  }
  int index = info[0].ToNumber().Int32Value();
  bool ret = SevlabMonitorBitsClear(index);
  return Napi::Boolean::New(env, ret);
}
Napi::Value SevlabNGetCVMonitorBitsDiff(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  if (info.Length() < 1 || !info[0].IsNumber()) {
    throw Napi::TypeError::New(env, "Expected one argument: [Number]");
  }
  int index = info[0].ToNumber().Int32Value();
  int diffN = 0;
  double* diffScore = SevlabMonitorBitsDiffScore(index, &diffN);
  if (!diffScore) return env.Undefined();
  Napi::Array diffs = Napi::Array::New(env, diffN);
  for (int i = 0; i < diffN; i++) {
     diffs[(uint32_t)i] = Napi::Number::New(env, diffScore[i]);
  }
  return diffs;
}
Napi::Value SevlabNAddCVMonitor(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  if (info.Length() < 7 || !info[0].IsNumber() || !info[1].IsNumber() || !info[2].IsNumber() || !info[3].IsNumber() || !info[4].IsNumber() || !info[5].IsNumber() || !info[6].IsNumber()) {
    throw Napi::TypeError::New(env, "Expected one argument: [Number],[Number],[Number],[Number],[Number],[Number],[Number],[Number]");
  }
  int x = info[0].ToNumber().Int32Value();
  int y = info[1].ToNumber().Int32Value();
  int w = info[2].ToNumber().Int32Value();
  int h = info[3].ToNumber().Int32Value();
  int cw = info[4].ToNumber().Int32Value();
  int ch = info[5].ToNumber().Int32Value();
  int diffCap = info[6].ToNumber().Int32Value();
  bool ret = SevlabMonitorAdd(x, y, w, h, cw, ch, diffCap);
  return Napi::Boolean::New(env, ret);
}
Napi::Value SevlabNClearCVMonitor(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  bool ret = SevlabMonitorClear();
  return Napi::Boolean::New(env, ret);
}
Napi::Value SevlabNIsPaused(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  return Napi::Boolean::New(env, sevlabCVPaused);
}
Napi::Value SevlabNStartCV(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  SevlabCVStart();
  return env.Undefined();
}
Napi::Value SevlabNStopCV(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  SevlabCVStop();
  return env.Undefined();
}
Napi::Value SevlabNCVDebug(const Napi::CallbackInfo &info)
{
  Napi::Env env = info.Env();
  Napi::Object obj = Napi::Object::New(env);
  obj.Set("monitorN", sevlabMonitorN);
  if (sevlabMonitorN > 0) {
     obj.Set("monitor0.bitsN", sevlabMonitors[0]->diffN);
     obj.Set("monitor0.bitsCap", sevlabMonitors[0]->diffCapN);
     obj.Set("monitor0.cx", (long)(sevlabMonitors[0]->rx));
     obj.Set("monitor0.cy", (long)(sevlabMonitors[0]->ry));
     obj.Set("monitor0.cw", (long)(sevlabMonitors[0]->rw));
     obj.Set("monitor0.ch", (long)(sevlabMonitors[0]->rh));
     obj.Set("monitor0.bits0.0", sevlabMonitors[0]->diffBits[0][0]);
     obj.Set("monitor0.bits0.1", sevlabMonitors[0]->diffBits[0][1]);
     obj.Set("monitor0.bits0.2", sevlabMonitors[0]->diffBits[0][2]);
     obj.Set("monitor0.bits0.3", sevlabMonitors[0]->diffBits[0][3]);
     obj.Set("sX", sevlabScaleX);
     obj.Set("sY", sevlabScaleY);
  }
  return obj;
}

Napi::Object Init(Napi::Env env, Napi::Object exports)
{
  exports["cvAddMonitorBits"] = Napi::Function::New(env, SevlabNAddCVMonitorBits);
  exports["cvClearMonitorBits"] = Napi::Function::New(env, SevlabNClearCVMonitorBits);
  exports["cvGetMonitorBitsDiff"] = Napi::Function::New(env, SevlabNGetCVMonitorBitsDiff);
  exports["cvAddMonitor"] = Napi::Function::New(env, SevlabNAddCVMonitor);
  exports["cvClearMonitor"] = Napi::Function::New(env, SevlabNClearCVMonitor);
  exports["cvIsPaused"] = Napi::Function::New(env, SevlabNIsPaused);
  exports["cvStart"] = Napi::Function::New(env, SevlabNStartCV);
  exports["cvStop"] = Napi::Function::New(env, SevlabNStopCV);
  exports["cvDebug"] = Napi::Function::New(env, SevlabNCVDebug);
  return exports;
}

NODE_API_MODULE(cv_core, Init)
