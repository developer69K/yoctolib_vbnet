'*********************************************************************
'*
'* $Id: yocto_carbondioxide.vb 15259 2014-03-06 10:21:05Z seb $
'*
'* Implements yFindCarbonDioxide(), the high-level API for CarbonDioxide functions
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

Module yocto_carbondioxide

    REM --- (YCarbonDioxide return codes)
    REM --- (end of YCarbonDioxide return codes)
  REM --- (YCarbonDioxide globals)

  Public Delegate Sub YCarbonDioxideValueCallback(ByVal func As YCarbonDioxide, ByVal value As String)
  Public Delegate Sub YCarbonDioxideTimedReportCallback(ByVal func As YCarbonDioxide, ByVal measure As YMeasure)
  REM --- (end of YCarbonDioxide globals)

  REM --- (YCarbonDioxide class start)

  '''*
  ''' <summary>
  '''   The Yoctopuce application programming interface allows you to read an instant
  '''   measure of the sensor, as well as the minimal and maximal values observed.
  ''' <para>
  ''' </para>
  ''' </summary>
  '''/
  Public Class YCarbonDioxide
    Inherits YSensor
    REM --- (end of YCarbonDioxide class start)

    REM --- (YCarbonDioxide definitions)
    REM --- (end of YCarbonDioxide definitions)

    REM --- (YCarbonDioxide attributes declaration)
    Protected _valueCallbackCarbonDioxide As YCarbonDioxideValueCallback
    Protected _timedReportCallbackCarbonDioxide As YCarbonDioxideTimedReportCallback
    REM --- (end of YCarbonDioxide attributes declaration)

    Public Sub New(ByVal func As String)
      MyBase.New(func)
      _classname = "CarbonDioxide"
      REM --- (YCarbonDioxide attributes initialization)
      _valueCallbackCarbonDioxide = Nothing
      _timedReportCallbackCarbonDioxide = Nothing
      REM --- (end of YCarbonDioxide attributes initialization)
    End Sub

    REM --- (YCarbonDioxide private methods declaration)

    Protected Overrides Function _parseAttr(ByRef member As TJSONRECORD) As Integer
      Return MyBase._parseAttr(member)
    End Function

    REM --- (end of YCarbonDioxide private methods declaration)

    REM --- (YCarbonDioxide public methods declaration)
    '''*
    ''' <summary>
    '''   Retrieves a CO2 sensor for a given identifier.
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
    '''   This function does not require that the CO2 sensor is online at the time
    '''   it is invoked. The returned object is nevertheless valid.
    '''   Use the method <c>YCarbonDioxide.isOnline()</c> to test if the CO2 sensor is
    '''   indeed online at a given time. In case of ambiguity when looking for
    '''   a CO2 sensor by logical name, no error is notified: the first instance
    '''   found is returned. The search is performed first by hardware name,
    '''   then by logical name.
    ''' </para>
    ''' </summary>
    ''' <param name="func">
    '''   a string that uniquely characterizes the CO2 sensor
    ''' </param>
    ''' <returns>
    '''   a <c>YCarbonDioxide</c> object allowing you to drive the CO2 sensor.
    ''' </returns>
    '''/
    Public Shared Function FindCarbonDioxide(func As String) As YCarbonDioxide
      Dim obj As YCarbonDioxide
      obj = CType(YFunction._FindFromCache("CarbonDioxide", func), YCarbonDioxide)
      If ((obj Is Nothing)) Then
        obj = New YCarbonDioxide(func)
        YFunction._AddToCache("CarbonDioxide", func, obj)
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
    Public Overloads Function registerValueCallback(callback As YCarbonDioxideValueCallback) As Integer
      Dim val As String
      If (Not (callback Is Nothing)) Then
        YFunction._UpdateValueCallbackList(Me, True)
      Else
        YFunction._UpdateValueCallbackList(Me, False)
      End If
      Me._valueCallbackCarbonDioxide = callback
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
      If (Not (Me._valueCallbackCarbonDioxide Is Nothing)) Then
        Me._valueCallbackCarbonDioxide(Me, value)
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
    Public Overloads Function registerTimedReportCallback(callback As YCarbonDioxideTimedReportCallback) As Integer
      If (Not (callback Is Nothing)) Then
        YFunction._UpdateTimedReportCallbackList(Me, True)
      Else
        YFunction._UpdateTimedReportCallbackList(Me, False)
      End If
      Me._timedReportCallbackCarbonDioxide = callback
      Return 0
    End Function

    Public Overrides Function _invokeTimedReportCallback(value As YMeasure) As Integer
      If (Not (Me._timedReportCallbackCarbonDioxide Is Nothing)) Then
        Me._timedReportCallbackCarbonDioxide(Me, value)
      Else
        MyBase._invokeTimedReportCallback(value)
      End If
      Return 0
    End Function


    '''*
    ''' <summary>
    '''   Continues the enumeration of CO2 sensors started using <c>yFirstCarbonDioxide()</c>.
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
    '''   a CO2 sensor currently online, or a <c>null</c> pointer
    '''   if there are no more CO2 sensors to enumerate.
    ''' </returns>
    '''/
    Public Function nextCarbonDioxide() As YCarbonDioxide
      Dim hwid As String = ""
      If (YISERR(_nextFunction(hwid))) Then
        Return Nothing
      End If
      If (hwid = "") Then
        Return Nothing
      End If
      Return YCarbonDioxide.FindCarbonDioxide(hwid)
    End Function

    '''*
    ''' <summary>
    '''   Starts the enumeration of CO2 sensors currently accessible.
    ''' <para>
    '''   Use the method <c>YCarbonDioxide.nextCarbonDioxide()</c> to iterate on
    '''   next CO2 sensors.
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
    '''   the first CO2 sensor currently online, or a <c>null</c> pointer
    '''   if there are none.
    ''' </returns>
    '''/
    Public Shared Function FirstCarbonDioxide() As YCarbonDioxide
      Dim v_fundescr(1) As YFUN_DESCR
      Dim dev As YDEV_DESCR
      Dim neededsize, err As Integer
      Dim serial, funcId, funcName, funcVal As String
      Dim errmsg As String = ""
      Dim size As Integer = Marshal.SizeOf(v_fundescr(0))
      Dim p As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr(0)))

      err = yapiGetFunctionsByClass("CarbonDioxide", 0, p, size, neededsize, errmsg)
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
      Return YCarbonDioxide.FindCarbonDioxide(serial + "." + funcId)
    End Function

    REM --- (end of YCarbonDioxide public methods declaration)

  End Class

  REM --- (CarbonDioxide functions)

  '''*
  ''' <summary>
  '''   Retrieves a CO2 sensor for a given identifier.
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
  '''   This function does not require that the CO2 sensor is online at the time
  '''   it is invoked. The returned object is nevertheless valid.
  '''   Use the method <c>YCarbonDioxide.isOnline()</c> to test if the CO2 sensor is
  '''   indeed online at a given time. In case of ambiguity when looking for
  '''   a CO2 sensor by logical name, no error is notified: the first instance
  '''   found is returned. The search is performed first by hardware name,
  '''   then by logical name.
  ''' </para>
  ''' </summary>
  ''' <param name="func">
  '''   a string that uniquely characterizes the CO2 sensor
  ''' </param>
  ''' <returns>
  '''   a <c>YCarbonDioxide</c> object allowing you to drive the CO2 sensor.
  ''' </returns>
  '''/
  Public Function yFindCarbonDioxide(ByVal func As String) As YCarbonDioxide
    Return YCarbonDioxide.FindCarbonDioxide(func)
  End Function

  '''*
  ''' <summary>
  '''   Starts the enumeration of CO2 sensors currently accessible.
  ''' <para>
  '''   Use the method <c>YCarbonDioxide.nextCarbonDioxide()</c> to iterate on
  '''   next CO2 sensors.
  ''' </para>
  ''' </summary>
  ''' <returns>
  '''   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
  '''   the first CO2 sensor currently online, or a <c>null</c> pointer
  '''   if there are none.
  ''' </returns>
  '''/
  Public Function yFirstCarbonDioxide() As YCarbonDioxide
    Return YCarbonDioxide.FirstCarbonDioxide()
  End Function


  REM --- (end of CarbonDioxide functions)

End Module
