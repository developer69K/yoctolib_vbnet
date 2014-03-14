'*********************************************************************
'*
'* $Id: yocto_power.vb 15259 2014-03-06 10:21:05Z seb $
'*
'* Implements yFindPower(), the high-level API for Power functions
'*
'* - - - - - - - - - License information: - - - - - - - - - 
'*
'*  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
'*
'*  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
'*  non-exclusive license to use, modify, copy and integrate this
'*  file into your software for the sole purpose of interfacing
'*  with Yoctopuce products.
'*
'*  You may reproduce and distribute copies of this file in
'*  source or object form, as long as the sole purpose of this
'*  code is to interface with Yoctopuce products. You must retain
'*  this notice in the distributed source file.
'*
'*  You should refer to Yoctopuce General Terms and Conditions
'*  for additional information regarding your rights and
'*  obligations.
'*
'*  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
'*  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING 
'*  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
'*  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
'*  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
'*  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
'*  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR 
'*  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT 
'*  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
'*  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
'*  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
'*  WARRANTY, OR OTHERWISE.
'*
'*********************************************************************/


Imports YDEV_DESCR = System.Int32
Imports YFUN_DESCR = System.Int32
Imports System.Runtime.InteropServices
Imports System.Text

Module yocto_power

    REM --- (YPower return codes)
    REM --- (end of YPower return codes)
  REM --- (YPower globals)

  Public Const Y_COSPHI_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_METER_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_METERTIMER_INVALID As Integer = YAPI.INVALID_UINT
  Public Delegate Sub YPowerValueCallback(ByVal func As YPower, ByVal value As String)
  Public Delegate Sub YPowerTimedReportCallback(ByVal func As YPower, ByVal measure As YMeasure)
  REM --- (end of YPower globals)

  REM --- (YPower class start)

  '''*
  ''' <summary>
  '''   The Yoctopuce application programming interface allows you to read an instant
  '''   measure of the sensor, as well as the minimal and maximal values observed.
  ''' <para>
  ''' </para>
  ''' </summary>
  '''/
  Public Class YPower
    Inherits YSensor
    REM --- (end of YPower class start)

    REM --- (YPower definitions)
    Public Const COSPHI_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const METER_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const METERTIMER_INVALID As Integer = YAPI.INVALID_UINT
    REM --- (end of YPower definitions)

    REM --- (YPower attributes declaration)
    Protected _cosPhi As Double
    Protected _meter As Double
    Protected _meterTimer As Integer
    Protected _valueCallbackPower As YPowerValueCallback
    Protected _timedReportCallbackPower As YPowerTimedReportCallback
    REM --- (end of YPower attributes declaration)

    Public Sub New(ByVal func As String)
      MyBase.New(func)
      _classname = "Power"
      REM --- (YPower attributes initialization)
      _cosPhi = COSPHI_INVALID
      _meter = METER_INVALID
      _meterTimer = METERTIMER_INVALID
      _valueCallbackPower = Nothing
      _timedReportCallbackPower = Nothing
      REM --- (end of YPower attributes initialization)
    End Sub

    REM --- (YPower private methods declaration)

    Protected Overrides Function _parseAttr(ByRef member As TJSONRECORD) As Integer
      If (member.name = "cosPhi") Then
        _cosPhi = member.ivalue / 65536.0
        Return 1
      End If
      If (member.name = "meter") Then
        _meter = member.ivalue / 65536.0
        Return 1
      End If
      If (member.name = "meterTimer") Then
        _meterTimer = CInt(member.ivalue)
        Return 1
      End If
      Return MyBase._parseAttr(member)
    End Function

    REM --- (end of YPower private methods declaration)

    REM --- (YPower public methods declaration)
    '''*
    ''' <summary>
    '''   Returns the power factor (the ratio between the real power consumed,
    '''   measured in W, and the apparent power provided, measured in VA).
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the power factor (the ratio between the real power consumed,
    '''   measured in W, and the apparent power provided, measured in VA)
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_COSPHI_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_cosPhi() As Double
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return COSPHI_INVALID
        End If
      End If
      Return Me._cosPhi
    End Function


    Public Function set_meter(ByVal newval As Double) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(Math.Round(newval * 65536.0)))
      Return _setAttr("meter", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the energy counter, maintained by the wattmeter by integrating the power consumption over time.
    ''' <para>
    '''   Note that this counter is reset at each start of the device.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the energy counter, maintained by the wattmeter by
    '''   integrating the power consumption over time
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_METER_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_meter() As Double
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return METER_INVALID
        End If
      End If
      Return Me._meter
    End Function

    '''*
    ''' <summary>
    '''   Returns the elapsed time since last energy counter reset, in seconds.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   an integer corresponding to the elapsed time since last energy counter reset, in seconds
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_METERTIMER_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_meterTimer() As Integer
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return METERTIMER_INVALID
        End If
      End If
      Return Me._meterTimer
    End Function

    '''*
    ''' <summary>
    '''   Retrieves a electrical power sensor for a given identifier.
    ''' <para>
    '''   The identifier can be specified using several formats:
    ''' </para>
    ''' <para>
    ''' </para>
    ''' <para>
    '''   - FunctionLogicalName
    ''' </para>
    ''' <para>
    '''   - ModuleSerialNumber.FunctionIdentifier
    ''' </para>
    ''' <para>
    '''   - ModuleSerialNumber.FunctionLogicalName
    ''' </para>
    ''' <para>
    '''   - ModuleLogicalName.FunctionIdentifier
    ''' </para>
    ''' <para>
    '''   - ModuleLogicalName.FunctionLogicalName
    ''' </para>
    ''' <para>
    ''' </para>
    ''' <para>
    '''   This function does not require that the electrical power sensor is online at the time
    '''   it is invoked. The returned object is nevertheless valid.
    '''   Use the method <c>YPower.isOnline()</c> to test if the electrical power sensor is
    '''   indeed online at a given time. In case of ambiguity when looking for
    '''   a electrical power sensor by logical name, no error is notified: the first instance
    '''   found is returned. The search is performed first by hardware name,
    '''   then by logical name.
    ''' </para>
    ''' </summary>
    ''' <param name="func">
    '''   a string that uniquely characterizes the electrical power sensor
    ''' </param>
    ''' <returns>
    '''   a <c>YPower</c> object allowing you to drive the electrical power sensor.
    ''' </returns>
    '''/
    Public Shared Function FindPower(func As String) As YPower
      Dim obj As YPower
      obj = CType(YFunction._FindFromCache("Power", func), YPower)
      If ((obj Is Nothing)) Then
        obj = New YPower(func)
        YFunction._AddToCache("Power", func, obj)
      End If
      Return obj
    End Function

    '''*
    ''' <summary>
    '''   Registers the callback function that is invoked on every change of advertised value.
    ''' <para>
    '''   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
    '''   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
    '''   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="callback">
    '''   the callback function to call, or a null pointer. The callback function should take two
    '''   arguments: the function object of which the value has changed, and the character string describing
    '''   the new advertised value.
    ''' @noreturn
    ''' </param>
    '''/
    Public Overloads Function registerValueCallback(callback As YPowerValueCallback) As Integer
      Dim val As String
      If (Not (callback Is Nothing)) Then
        YFunction._UpdateValueCallbackList(Me, True)
      Else
        YFunction._UpdateValueCallbackList(Me, False)
      End If
      Me._valueCallbackPower = callback
      REM // Immediately invoke value callback with current value
      If (Not (callback Is Nothing) And Me.isOnline()) Then
        val = Me._advertisedValue
        If (Not (val = "")) Then
          Me._invokeValueCallback(val)
        End If
      End If
      Return 0
    End Function

    Public Overrides Function _invokeValueCallback(value As String) As Integer
      If (Not (Me._valueCallbackPower Is Nothing)) Then
        Me._valueCallbackPower(Me, value)
      Else
        MyBase._invokeValueCallback(value)
      End If
      Return 0
    End Function

    '''*
    ''' <summary>
    '''   Registers the callback function that is invoked on every periodic timed notification.
    ''' <para>
    '''   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
    '''   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
    '''   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="callback">
    '''   the callback function to call, or a null pointer. The callback function should take two
    '''   arguments: the function object of which the value has changed, and an YMeasure object describing
    '''   the new advertised value.
    ''' @noreturn
    ''' </param>
    '''/
    Public Overloads Function registerTimedReportCallback(callback As YPowerTimedReportCallback) As Integer
      If (Not (callback Is Nothing)) Then
        YFunction._UpdateTimedReportCallbackList(Me, True)
      Else
        YFunction._UpdateTimedReportCallbackList(Me, False)
      End If
      Me._timedReportCallbackPower = callback
      Return 0
    End Function

    Public Overrides Function _invokeTimedReportCallback(value As YMeasure) As Integer
      If (Not (Me._timedReportCallbackPower Is Nothing)) Then
        Me._timedReportCallbackPower(Me, value)
      Else
        MyBase._invokeTimedReportCallback(value)
      End If
      Return 0
    End Function

    '''*
    ''' <summary>
    '''   Resets the energy counter.
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Overridable Function reset() As Integer
      Return Me.set_meter(0)
    End Function


    '''*
    ''' <summary>
    '''   Continues the enumeration of electrical power sensors started using <c>yFirstPower()</c>.
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YPower</c> object, corresponding to
    '''   a electrical power sensor currently online, or a <c>null</c> pointer
    '''   if there are no more electrical power sensors to enumerate.
    ''' </returns>
    '''/
    Public Function nextPower() As YPower
      Dim hwid As String = ""
      If (YISERR(_nextFunction(hwid))) Then
        Return Nothing
      End If
      If (hwid = "") Then
        Return Nothing
      End If
      Return YPower.FindPower(hwid)
    End Function

    '''*
    ''' <summary>
    '''   Starts the enumeration of electrical power sensors currently accessible.
    ''' <para>
    '''   Use the method <c>YPower.nextPower()</c> to iterate on
    '''   next electrical power sensors.
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YPower</c> object, corresponding to
    '''   the first electrical power sensor currently online, or a <c>null</c> pointer
    '''   if there are none.
    ''' </returns>
    '''/
    Public Shared Function FirstPower() As YPower
      Dim v_fundescr(1) As YFUN_DESCR
      Dim dev As YDEV_DESCR
      Dim neededsize, err As Integer
      Dim serial, funcId, funcName, funcVal As String
      Dim errmsg As String = ""
      Dim size As Integer = Marshal.SizeOf(v_fundescr(0))
      Dim p As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr(0)))

      err = yapiGetFunctionsByClass("Power", 0, p, size, neededsize, errmsg)
      Marshal.Copy(p, v_fundescr, 0, 1)
      Marshal.FreeHGlobal(p)

      If (YISERR(err) Or (neededsize = 0)) Then
        Return Nothing
      End If
      serial = ""
      funcId = ""
      funcName = ""
      funcVal = ""
      errmsg = ""
      If (YISERR(yapiGetFunctionInfo(v_fundescr(0), dev, serial, funcId, funcName, funcVal, errmsg))) Then
        Return Nothing
      End If
      Return YPower.FindPower(serial + "." + funcId)
    End Function

    REM --- (end of YPower public methods declaration)

  End Class

  REM --- (Power functions)

  '''*
  ''' <summary>
  '''   Retrieves a electrical power sensor for a given identifier.
  ''' <para>
  '''   The identifier can be specified using several formats:
  ''' </para>
  ''' <para>
  ''' </para>
  ''' <para>
  '''   - FunctionLogicalName
  ''' </para>
  ''' <para>
  '''   - ModuleSerialNumber.FunctionIdentifier
  ''' </para>
  ''' <para>
  '''   - ModuleSerialNumber.FunctionLogicalName
  ''' </para>
  ''' <para>
  '''   - ModuleLogicalName.FunctionIdentifier
  ''' </para>
  ''' <para>
  '''   - ModuleLogicalName.FunctionLogicalName
  ''' </para>
  ''' <para>
  ''' </para>
  ''' <para>
  '''   This function does not require that the electrical power sensor is online at the time
  '''   it is invoked. The returned object is nevertheless valid.
  '''   Use the method <c>YPower.isOnline()</c> to test if the electrical power sensor is
  '''   indeed online at a given time. In case of ambiguity when looking for
  '''   a electrical power sensor by logical name, no error is notified: the first instance
  '''   found is returned. The search is performed first by hardware name,
  '''   then by logical name.
  ''' </para>
  ''' </summary>
  ''' <param name="func">
  '''   a string that uniquely characterizes the electrical power sensor
  ''' </param>
  ''' <returns>
  '''   a <c>YPower</c> object allowing you to drive the electrical power sensor.
  ''' </returns>
  '''/
  Public Function yFindPower(ByVal func As String) As YPower
    Return YPower.FindPower(func)
  End Function

  '''*
  ''' <summary>
  '''   Starts the enumeration of electrical power sensors currently accessible.
  ''' <para>
  '''   Use the method <c>YPower.nextPower()</c> to iterate on
  '''   next electrical power sensors.
  ''' </para>
  ''' </summary>
  ''' <returns>
  '''   a pointer to a <c>YPower</c> object, corresponding to
  '''   the first electrical power sensor currently online, or a <c>null</c> pointer
  '''   if there are none.
  ''' </returns>
  '''/
  Public Function yFirstPower() As YPower
    Return YPower.FirstPower()
  End Function


  REM --- (end of Power functions)

End Module
