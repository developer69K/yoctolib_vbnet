'*********************************************************************
'*
'* $Id: yocto_powersupply.vb 28740 2017-10-03 08:09:13Z seb $
'*
'* Implements yFindPowerSupply(), the high-level API for PowerSupply functions
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

Module yocto_powersupply

    REM --- (YPowerSupply return codes)
    REM --- (end of YPowerSupply return codes)
    REM --- (YPowerSupply dlldef)
    REM --- (end of YPowerSupply dlldef)
  REM --- (YPowerSupply globals)

  Public Const Y_VOLTAGESETPOINT_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_CURRENTLIMIT_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_POWEROUTPUT_OFF As Integer = 0
  Public Const Y_POWEROUTPUT_ON As Integer = 1
  Public Const Y_POWEROUTPUT_INVALID As Integer = -1
  Public Const Y_VOLTAGESENSE_INT As Integer = 0
  Public Const Y_VOLTAGESENSE_EXT As Integer = 1
  Public Const Y_VOLTAGESENSE_INVALID As Integer = -1
  Public Const Y_MEASUREDVOLTAGE_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_MEASUREDCURRENT_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_INPUTVOLTAGE_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_VINT_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_LDOTEMPERATURE_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_VOLTAGETRANSITION_INVALID As String = YAPI.INVALID_STRING
  Public Const Y_VOLTAGEATSTARTUP_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_CURRENTATSTARTUP_INVALID As Double = YAPI.INVALID_DOUBLE
  Public Const Y_COMMAND_INVALID As String = YAPI.INVALID_STRING
  Public Delegate Sub YPowerSupplyValueCallback(ByVal func As YPowerSupply, ByVal value As String)
  Public Delegate Sub YPowerSupplyTimedReportCallback(ByVal func As YPowerSupply, ByVal measure As YMeasure)
  REM --- (end of YPowerSupply globals)

  REM --- (YPowerSupply class start)

  '''*
  ''' <summary>
  '''   The Yoctopuce application programming interface allows you to change the voltage set point,
  '''   the current limit and the enable/disable the output.
  ''' <para>
  ''' </para>
  ''' </summary>
  '''/
  Public Class YPowerSupply
    Inherits YFunction
    REM --- (end of YPowerSupply class start)

    REM --- (YPowerSupply definitions)
    Public Const VOLTAGESETPOINT_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const CURRENTLIMIT_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const POWEROUTPUT_OFF As Integer = 0
    Public Const POWEROUTPUT_ON As Integer = 1
    Public Const POWEROUTPUT_INVALID As Integer = -1
    Public Const VOLTAGESENSE_INT As Integer = 0
    Public Const VOLTAGESENSE_EXT As Integer = 1
    Public Const VOLTAGESENSE_INVALID As Integer = -1
    Public Const MEASUREDVOLTAGE_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const MEASUREDCURRENT_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const INPUTVOLTAGE_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const VINT_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const LDOTEMPERATURE_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const VOLTAGETRANSITION_INVALID As String = YAPI.INVALID_STRING
    Public Const VOLTAGEATSTARTUP_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const CURRENTATSTARTUP_INVALID As Double = YAPI.INVALID_DOUBLE
    Public Const COMMAND_INVALID As String = YAPI.INVALID_STRING
    REM --- (end of YPowerSupply definitions)

    REM --- (YPowerSupply attributes declaration)
    Protected _voltageSetPoint As Double
    Protected _currentLimit As Double
    Protected _powerOutput As Integer
    Protected _voltageSense As Integer
    Protected _measuredVoltage As Double
    Protected _measuredCurrent As Double
    Protected _inputVoltage As Double
    Protected _vInt As Double
    Protected _ldoTemperature As Double
    Protected _voltageTransition As String
    Protected _voltageAtStartUp As Double
    Protected _currentAtStartUp As Double
    Protected _command As String
    Protected _valueCallbackPowerSupply As YPowerSupplyValueCallback
    REM --- (end of YPowerSupply attributes declaration)

    Public Sub New(ByVal func As String)
      MyBase.New(func)
      _classname = "PowerSupply"
      REM --- (YPowerSupply attributes initialization)
      _voltageSetPoint = VOLTAGESETPOINT_INVALID
      _currentLimit = CURRENTLIMIT_INVALID
      _powerOutput = POWEROUTPUT_INVALID
      _voltageSense = VOLTAGESENSE_INVALID
      _measuredVoltage = MEASUREDVOLTAGE_INVALID
      _measuredCurrent = MEASUREDCURRENT_INVALID
      _inputVoltage = INPUTVOLTAGE_INVALID
      _vInt = VINT_INVALID
      _ldoTemperature = LDOTEMPERATURE_INVALID
      _voltageTransition = VOLTAGETRANSITION_INVALID
      _voltageAtStartUp = VOLTAGEATSTARTUP_INVALID
      _currentAtStartUp = CURRENTATSTARTUP_INVALID
      _command = COMMAND_INVALID
      _valueCallbackPowerSupply = Nothing
      REM --- (end of YPowerSupply attributes initialization)
    End Sub

    REM --- (YPowerSupply private methods declaration)

    Protected Overrides Function _parseAttr(ByRef json_val As YJSONObject) As Integer
      If json_val.has("voltageSetPoint") Then
        _voltageSetPoint = Math.Round(json_val.getDouble("voltageSetPoint") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("currentLimit") Then
        _currentLimit = Math.Round(json_val.getDouble("currentLimit") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("powerOutput") Then
        If (json_val.getInt("powerOutput") > 0) Then _powerOutput = 1 Else _powerOutput = 0
      End If
      If json_val.has("voltageSense") Then
        _voltageSense = CInt(json_val.getLong("voltageSense"))
      End If
      If json_val.has("measuredVoltage") Then
        _measuredVoltage = Math.Round(json_val.getDouble("measuredVoltage") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("measuredCurrent") Then
        _measuredCurrent = Math.Round(json_val.getDouble("measuredCurrent") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("inputVoltage") Then
        _inputVoltage = Math.Round(json_val.getDouble("inputVoltage") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("vInt") Then
        _vInt = Math.Round(json_val.getDouble("vInt") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("ldoTemperature") Then
        _ldoTemperature = Math.Round(json_val.getDouble("ldoTemperature") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("voltageTransition") Then
        _voltageTransition = json_val.getString("voltageTransition")
      End If
      If json_val.has("voltageAtStartUp") Then
        _voltageAtStartUp = Math.Round(json_val.getDouble("voltageAtStartUp") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("currentAtStartUp") Then
        _currentAtStartUp = Math.Round(json_val.getDouble("currentAtStartUp") * 1000.0 / 65536.0) / 1000.0
      End If
      If json_val.has("command") Then
        _command = json_val.getString("command")
      End If
      Return MyBase._parseAttr(json_val)
    End Function

    REM --- (end of YPowerSupply private methods declaration)

    REM --- (YPowerSupply public methods declaration)

    '''*
    ''' <summary>
    '''   Changes the voltage set point, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   a floating point number corresponding to the voltage set point, in V
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_voltageSetPoint(ByVal newval As Double) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(Math.Round(newval * 65536.0)))
      Return _setAttr("voltageSetPoint", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the voltage set point, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the voltage set point, in V
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_VOLTAGESETPOINT_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_voltageSetPoint() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return VOLTAGESETPOINT_INVALID
        End If
      End If
      res = Me._voltageSetPoint
      Return res
    End Function


    '''*
    ''' <summary>
    '''   Changes the current limit, in mA.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   a floating point number corresponding to the current limit, in mA
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_currentLimit(ByVal newval As Double) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(Math.Round(newval * 65536.0)))
      Return _setAttr("currentLimit", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the current limit, in mA.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the current limit, in mA
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_CURRENTLIMIT_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_currentLimit() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return CURRENTLIMIT_INVALID
        End If
      End If
      res = Me._currentLimit
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Returns the power supply output switch state.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   either <c>Y_POWEROUTPUT_OFF</c> or <c>Y_POWEROUTPUT_ON</c>, according to the power supply output switch state
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_POWEROUTPUT_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_powerOutput() As Integer
      Dim res As Integer
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return POWEROUTPUT_INVALID
        End If
      End If
      res = Me._powerOutput
      Return res
    End Function


    '''*
    ''' <summary>
    '''   Changes the power supply output switch state.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   either <c>Y_POWEROUTPUT_OFF</c> or <c>Y_POWEROUTPUT_ON</c>, according to the power supply output switch state
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_powerOutput(ByVal newval As Integer) As Integer
      Dim rest_val As String
      If (newval > 0) Then rest_val = "1" Else rest_val = "0"
      Return _setAttr("powerOutput", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the output voltage control point.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   either <c>Y_VOLTAGESENSE_INT</c> or <c>Y_VOLTAGESENSE_EXT</c>, according to the output voltage control point
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_VOLTAGESENSE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_voltageSense() As Integer
      Dim res As Integer
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return VOLTAGESENSE_INVALID
        End If
      End If
      res = Me._voltageSense
      Return res
    End Function


    '''*
    ''' <summary>
    '''   Changes the voltage control point.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   either <c>Y_VOLTAGESENSE_INT</c> or <c>Y_VOLTAGESENSE_EXT</c>, according to the voltage control point
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_voltageSense(ByVal newval As Integer) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(newval))
      Return _setAttr("voltageSense", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the measured output voltage, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the measured output voltage, in V
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_MEASUREDVOLTAGE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_measuredVoltage() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return MEASUREDVOLTAGE_INVALID
        End If
      End If
      res = Me._measuredVoltage
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Returns the measured output current, in mA.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the measured output current, in mA
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_MEASUREDCURRENT_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_measuredCurrent() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return MEASUREDCURRENT_INVALID
        End If
      End If
      res = Me._measuredCurrent
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Returns the measured input voltage, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the measured input voltage, in V
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_INPUTVOLTAGE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_inputVoltage() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return INPUTVOLTAGE_INVALID
        End If
      End If
      res = Me._inputVoltage
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Returns the internal voltage, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the internal voltage, in V
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_VINT_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_vInt() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return VINT_INVALID
        End If
      End If
      res = Me._vInt
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Returns the LDO temperature, in Celsius.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the LDO temperature, in Celsius
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_LDOTEMPERATURE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_ldoTemperature() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return LDOTEMPERATURE_INVALID
        End If
      End If
      res = Me._ldoTemperature
      Return res
    End Function

    Public Function get_voltageTransition() As String
      Dim res As String
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return VOLTAGETRANSITION_INVALID
        End If
      End If
      res = Me._voltageTransition
      Return res
    End Function


    Public Function set_voltageTransition(ByVal newval As String) As Integer
      Dim rest_val As String
      rest_val = newval
      Return _setAttr("voltageTransition", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Changes the voltage set point at device start up.
    ''' <para>
    '''   Remember to call the matching
    '''   module <c>saveToFlash()</c> method, otherwise this call has no effect.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   a floating point number corresponding to the voltage set point at device start up
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_voltageAtStartUp(ByVal newval As Double) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(Math.Round(newval * 65536.0)))
      Return _setAttr("voltageAtStartUp", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the selected voltage set point at device startup, in V.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the selected voltage set point at device startup, in V
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_VOLTAGEATSTARTUP_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_voltageAtStartUp() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return VOLTAGEATSTARTUP_INVALID
        End If
      End If
      res = Me._voltageAtStartUp
      Return res
    End Function


    '''*
    ''' <summary>
    '''   Changes the current limit at device start up.
    ''' <para>
    '''   Remember to call the matching
    '''   module <c>saveToFlash()</c> method, otherwise this call has no effect.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   a floating point number corresponding to the current limit at device start up
    ''' </param>
    ''' <para>
    ''' </para>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> if the call succeeds.
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns a negative error code.
    ''' </para>
    '''/
    Public Function set_currentAtStartUp(ByVal newval As Double) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(Math.Round(newval * 65536.0)))
      Return _setAttr("currentAtStartUp", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Returns the selected current limit at device startup, in mA.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a floating point number corresponding to the selected current limit at device startup, in mA
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_CURRENTATSTARTUP_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_currentAtStartUp() As Double
      Dim res As Double = 0
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return CURRENTATSTARTUP_INVALID
        End If
      End If
      res = Me._currentAtStartUp
      Return res
    End Function

    Public Function get_command() As String
      Dim res As String
      If (Me._cacheExpiration <= YAPI.GetTickCount()) Then
        If (Me.load(YAPI.DefaultCacheValidity) <> YAPI.SUCCESS) Then
          Return COMMAND_INVALID
        End If
      End If
      res = Me._command
      Return res
    End Function


    Public Function set_command(ByVal newval As String) As Integer
      Dim rest_val As String
      rest_val = newval
      Return _setAttr("command", rest_val)
    End Function
    '''*
    ''' <summary>
    '''   Retrieves a regulated power supply for a given identifier.
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
    '''   This function does not require that the regulated power supply is online at the time
    '''   it is invoked. The returned object is nevertheless valid.
    '''   Use the method <c>YPowerSupply.isOnline()</c> to test if the regulated power supply is
    '''   indeed online at a given time. In case of ambiguity when looking for
    '''   a regulated power supply by logical name, no error is notified: the first instance
    '''   found is returned. The search is performed first by hardware name,
    '''   then by logical name.
    ''' </para>
    ''' <para>
    '''   If a call to this object's is_online() method returns FALSE although
    '''   you are certain that the matching device is plugged, make sure that you did
    '''   call registerHub() at application initialization time.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="func">
    '''   a string that uniquely characterizes the regulated power supply
    ''' </param>
    ''' <returns>
    '''   a <c>YPowerSupply</c> object allowing you to drive the regulated power supply.
    ''' </returns>
    '''/
    Public Shared Function FindPowerSupply(func As String) As YPowerSupply
      Dim obj As YPowerSupply
      obj = CType(YFunction._FindFromCache("PowerSupply", func), YPowerSupply)
      If ((obj Is Nothing)) Then
        obj = New YPowerSupply(func)
        YFunction._AddToCache("PowerSupply", func, obj)
      End If
      Return obj
    End Function

    '''*
    ''' <summary>
    '''   Registers the callback function that is invoked on every change of advertised value.
    ''' <para>
    '''   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
    '''   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
    '''   one of these two functions periodically. To unregister a callback, pass a Nothing pointer as argument.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="callback">
    '''   the callback function to call, or a Nothing pointer. The callback function should take two
    '''   arguments: the function object of which the value has changed, and the character string describing
    '''   the new advertised value.
    ''' @noreturn
    ''' </param>
    '''/
    Public Overloads Function registerValueCallback(callback As YPowerSupplyValueCallback) As Integer
      Dim val As String
      If (Not (callback Is Nothing)) Then
        YFunction._UpdateValueCallbackList(Me, True)
      Else
        YFunction._UpdateValueCallbackList(Me, False)
      End If
      Me._valueCallbackPowerSupply = callback
      REM // Immediately invoke value callback with current value
      If (Not (callback Is Nothing) AndAlso Me.isOnline()) Then
        val = Me._advertisedValue
        If (Not (val = "")) Then
          Me._invokeValueCallback(val)
        End If
      End If
      Return 0
    End Function

    Public Overrides Function _invokeValueCallback(value As String) As Integer
      If (Not (Me._valueCallbackPowerSupply Is Nothing)) Then
        Me._valueCallbackPowerSupply(Me, value)
      Else
        MyBase._invokeValueCallback(value)
      End If
      Return 0
    End Function

    '''*
    ''' <summary>
    '''   Performs a smooth transistion of output voltage.
    ''' <para>
    '''   Any explicit voltage
    '''   change cancels any ongoing transition process.
    ''' </para>
    ''' </summary>
    ''' <param name="V_target">
    '''   new output voltage value at the end of the transition
    '''   (floating-point number, representing the end voltage in V)
    ''' </param>
    ''' <param name="ms_duration">
    '''   total duration of the transition, in milliseconds
    ''' </param>
    ''' <returns>
    '''   <c>YAPI_SUCCESS</c> when the call succeeds.
    ''' </returns>
    '''/
    Public Overridable Function voltageMove(V_target As Double, ms_duration As Integer) As Integer
      Dim newval As String
      If (V_target < 0.0) Then
        V_target  = 0.0
      End If
      newval = "" + Convert.ToString( CType(Math.Round(V_target*65536), Integer)) + ":" + Convert.ToString(ms_duration)

      Return Me.set_voltageTransition(newval)
    End Function


    '''*
    ''' <summary>
    '''   Continues the enumeration of regulated power supplies started using <c>yFirstPowerSupply()</c>.
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YPowerSupply</c> object, corresponding to
    '''   a regulated power supply currently online, or a <c>Nothing</c> pointer
    '''   if there are no more regulated power supplies to enumerate.
    ''' </returns>
    '''/
    Public Function nextPowerSupply() As YPowerSupply
      Dim hwid As String = ""
      If (YISERR(_nextFunction(hwid))) Then
        Return Nothing
      End If
      If (hwid = "") Then
        Return Nothing
      End If
      Return YPowerSupply.FindPowerSupply(hwid)
    End Function

    '''*
    ''' <summary>
    '''   Starts the enumeration of regulated power supplies currently accessible.
    ''' <para>
    '''   Use the method <c>YPowerSupply.nextPowerSupply()</c> to iterate on
    '''   next regulated power supplies.
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YPowerSupply</c> object, corresponding to
    '''   the first regulated power supply currently online, or a <c>Nothing</c> pointer
    '''   if there are none.
    ''' </returns>
    '''/
    Public Shared Function FirstPowerSupply() As YPowerSupply
      Dim v_fundescr(1) As YFUN_DESCR
      Dim dev As YDEV_DESCR
      Dim neededsize, err As Integer
      Dim serial, funcId, funcName, funcVal As String
      Dim errmsg As String = ""
      Dim size As Integer = Marshal.SizeOf(v_fundescr(0))
      Dim p As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr(0)))

      err = yapiGetFunctionsByClass("PowerSupply", 0, p, size, neededsize, errmsg)
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
      Return YPowerSupply.FindPowerSupply(serial + "." + funcId)
    End Function

    REM --- (end of YPowerSupply public methods declaration)

  End Class

  REM --- (YPowerSupply functions)

  '''*
  ''' <summary>
  '''   Retrieves a regulated power supply for a given identifier.
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
  '''   This function does not require that the regulated power supply is online at the time
  '''   it is invoked. The returned object is nevertheless valid.
  '''   Use the method <c>YPowerSupply.isOnline()</c> to test if the regulated power supply is
  '''   indeed online at a given time. In case of ambiguity when looking for
  '''   a regulated power supply by logical name, no error is notified: the first instance
  '''   found is returned. The search is performed first by hardware name,
  '''   then by logical name.
  ''' </para>
  ''' <para>
  '''   If a call to this object's is_online() method returns FALSE although
  '''   you are certain that the matching device is plugged, make sure that you did
  '''   call registerHub() at application initialization time.
  ''' </para>
  ''' <para>
  ''' </para>
  ''' </summary>
  ''' <param name="func">
  '''   a string that uniquely characterizes the regulated power supply
  ''' </param>
  ''' <returns>
  '''   a <c>YPowerSupply</c> object allowing you to drive the regulated power supply.
  ''' </returns>
  '''/
  Public Function yFindPowerSupply(ByVal func As String) As YPowerSupply
    Return YPowerSupply.FindPowerSupply(func)
  End Function

  '''*
  ''' <summary>
  '''   Starts the enumeration of regulated power supplies currently accessible.
  ''' <para>
  '''   Use the method <c>YPowerSupply.nextPowerSupply()</c> to iterate on
  '''   next regulated power supplies.
  ''' </para>
  ''' </summary>
  ''' <returns>
  '''   a pointer to a <c>YPowerSupply</c> object, corresponding to
  '''   the first regulated power supply currently online, or a <c>Nothing</c> pointer
  '''   if there are none.
  ''' </returns>
  '''/
  Public Function yFirstPowerSupply() As YPowerSupply
    Return YPowerSupply.FirstPowerSupply()
  End Function


  REM --- (end of YPowerSupply functions)

End Module