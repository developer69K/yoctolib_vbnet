'*********************************************************************
'*
'* $Id: yocto_servo.vb 10401 2013-03-17 13:17:31Z martinm $
'*
'* Implements yFindServo(), the high-level API for Servo functions
'*
'* - - - - - - - - - License information: - - - - - - - - - 
'*
'* Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
'*
'* 1) If you have obtained this file from www.yoctopuce.com,
'*    Yoctopuce Sarl licenses to you (hereafter Licensee) the
'*    right to use, modify, copy, and integrate this source file
'*    into your own solution for the sole purpose of interfacing
'*    a Yoctopuce product with Licensee's solution.
'*
'*    The use of this file and all relationship between Yoctopuce 
'*    and Licensee are governed by Yoctopuce General Terms and 
'*    Conditions.
'*
'*    THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
'*    WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING 
'*    WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS 
'*    FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
'*    EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
'*    INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, 
'*    COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR 
'*    SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT 
'*    LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
'*    CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
'*    BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
'*    WARRANTY, OR OTHERWISE.
'*
'* 2) If your intent is not to interface with Yoctopuce products,
'*    you are not entitled to use, read or create any derived
'*    material from this source file.
'*
'*********************************************************************/


Imports YDEV_DESCR = System.Int32
Imports YFUN_DESCR = System.Int32
Imports System.Runtime.InteropServices
Imports System.Text

Module yocto_servo

  REM --- (YServo definitions)

  Public Delegate Sub UpdateCallback(ByVal func As YServo, ByVal value As String)

Public Class YServoMove
  Public target As System.Int64 = YAPI.INVALID_LONG
  Public ms As System.Int64 = YAPI.INVALID_LONG
  Public moving As System.Int64 = YAPI.INVALID_LONG
End Class


  Public Const Y_LOGICALNAME_INVALID As String = YAPI.INVALID_STRING
  Public Const Y_ADVERTISEDVALUE_INVALID As String = YAPI.INVALID_STRING
  Public Const Y_POSITION_INVALID As Integer = YAPI.INVALID_INT
  Public Const Y_RANGE_INVALID As Integer = YAPI.INVALID_UNSIGNED
  Public Const Y_NEUTRAL_INVALID As Integer = YAPI.INVALID_UNSIGNED

  Public Y_MOVE_INVALID As YServoMove

  REM --- (end of YServo definitions)

  REM --- (YServo implementation)

  Private _ServoCache As New Hashtable()
  Private _callback As UpdateCallback

  '''*
  ''' <summary>
  '''   Yoctopuce application programming interface allows you not only to move
  '''   a servo to a given position, but also to specify the time interval
  '''   in which the move should be performed.
  ''' <para>
  '''   This makes it possible to
  '''   synchronize two servos involved in a same move.
  ''' </para>
  ''' </summary>
  '''/
  Public Class YServo
    Inherits YFunction
    Public Const LOGICALNAME_INVALID As String = YAPI.INVALID_STRING
    Public Const ADVERTISEDVALUE_INVALID As String = YAPI.INVALID_STRING
    Public Const POSITION_INVALID As Integer = YAPI.INVALID_INT
    Public Const RANGE_INVALID As Integer = YAPI.INVALID_UNSIGNED
    Public Const NEUTRAL_INVALID As Integer = YAPI.INVALID_UNSIGNED

    Protected _logicalName As String
    Protected _advertisedValue As String
    Protected _position As Long
    Protected _range As Long
    Protected _neutral As Long
    Protected _move As YServoMove

    Public Sub New(ByVal func As String)
      MyBase.new("Servo", func)
      _logicalName = Y_LOGICALNAME_INVALID
      _advertisedValue = Y_ADVERTISEDVALUE_INVALID
      _position = Y_POSITION_INVALID
      _range = Y_RANGE_INVALID
      _neutral = Y_NEUTRAL_INVALID
      _move = New YServoMove()
    End Sub

    Protected Overrides Function _parse(ByRef j As TJSONRECORD) As Integer
      Dim member As TJSONRECORD
      Dim i As Integer
      If (j.recordtype <> TJSONRECORDTYPE.JSON_STRUCT) Then
        Return -1
      End If
      For i = 0 To j.membercount - 1
        member = j.members(i)
        If (member.name = "logicalName") Then
          _logicalName = member.svalue
        ElseIf (member.name = "advertisedValue") Then
          _advertisedValue = member.svalue
        ElseIf (member.name = "position") Then
          _position = member.ivalue
        ElseIf (member.name = "range") Then
          _range = CLng(member.ivalue)
        ElseIf (member.name = "neutral") Then
          _neutral = CLng(member.ivalue)
        ElseIf (member.name = "move") Then
          If (member.recordtype <> TJSONRECORDTYPE.JSON_STRUCT) Then 
             _parse = -1
             Exit Function
          End If
          Dim submemb As TJSONRECORD
          Dim l As Integer
          For l=0 To member.membercount-1
             submemb = member.members(l)
             If (submemb.name = "moving") Then
                _move.moving = submemb.ivalue
             ElseIf (submemb.name = "target") Then
                _move.target = submemb.ivalue
             ElseIf (submemb.name = "ms") Then
                _move.ms = submemb.ivalue
             End If
          Next l
        End If
      Next i
      Return 0
    End Function

    '''*
    ''' <summary>
    '''   Returns the logical name of the servo.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a string corresponding to the logical name of the servo
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_LOGICALNAME_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_logicalName() As String
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_LOGICALNAME_INVALID
        End If
      End If
      Return _logicalName
    End Function

    '''*
    ''' <summary>
    '''   Changes the logical name of the servo.
    ''' <para>
    '''   You can use <c>yCheckLogicalName()</c>
    '''   prior to this call to make sure that your parameter is valid.
    '''   Remember to call the <c>saveToFlash()</c> method of the module if the
    '''   modification must be kept.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   a string corresponding to the logical name of the servo
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
    Public Function set_logicalName(ByVal newval As String) As Integer
      Dim rest_val As String
      rest_val = newval
      Return _setAttr("logicalName", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Returns the current value of the servo (no more than 6 characters).
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a string corresponding to the current value of the servo (no more than 6 characters)
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_ADVERTISEDVALUE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_advertisedValue() As String
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_ADVERTISEDVALUE_INVALID
        End If
      End If
      Return _advertisedValue
    End Function

    '''*
    ''' <summary>
    '''   Returns the current servo position.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   an integer corresponding to the current servo position
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_POSITION_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_position() As Integer
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_POSITION_INVALID
        End If
      End If
      Return CType(_position,Integer)
    End Function

    '''*
    ''' <summary>
    '''   Changes immediately the servo driving position.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   an integer corresponding to immediately the servo driving position
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
    Public Function set_position(ByVal newval As Integer) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(newval))
      Return _setAttr("position", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Returns the current range of use of the servo.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   an integer corresponding to the current range of use of the servo
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_RANGE_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_range() As Integer
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_RANGE_INVALID
        End If
      End If
      Return CType(_range,Integer)
    End Function

    '''*
    ''' <summary>
    '''   Changes the range of use of the servo, specified in per cents.
    ''' <para>
    '''   A range of 100% corresponds to a standard control signal, that varies
    '''   from 1 [ms] to 2 [ms], When using a servo that supports a double range,
    '''   from 0.5 [ms] to 2.5 [ms], you can select a range of 200%.
    '''   Be aware that using a range higher than what is supported by the servo
    '''   is likely to damage the servo.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   an integer corresponding to the range of use of the servo, specified in per cents
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
    Public Function set_range(ByVal newval As Integer) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(newval))
      Return _setAttr("range", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Returns the duration in microseconds of a neutral pulse for the servo.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   an integer corresponding to the duration in microseconds of a neutral pulse for the servo
    ''' </returns>
    ''' <para>
    '''   On failure, throws an exception or returns <c>Y_NEUTRAL_INVALID</c>.
    ''' </para>
    '''/
    Public Function get_neutral() As Integer
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_NEUTRAL_INVALID
        End If
      End If
      Return CType(_neutral,Integer)
    End Function

    '''*
    ''' <summary>
    '''   Changes the duration of the pulse corresponding to the neutral position of the servo.
    ''' <para>
    '''   The duration is specified in microseconds, and the standard value is 1500 [us].
    '''   This setting makes it possible to shift the range of use of the servo.
    '''   Be aware that using a range higher than what is supported by the servo is
    '''   likely to damage the servo.
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="newval">
    '''   an integer corresponding to the duration of the pulse corresponding to the neutral position of the servo
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
    Public Function set_neutral(ByVal newval As Integer) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(newval))
      Return _setAttr("neutral", rest_val)
    End Function

    Public Function get_move() As YServoMove
      If (_cacheExpiration <= YAPI.GetTickCount()) Then
        If (YISERR(load(YAPI.DefaultCacheValidity))) Then
          Return Y_MOVE_INVALID
        End If
      End If
      Return _move
    End Function

    Public Function set_move(ByVal newval As YServoMove) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(newval.target))+":"+Ltrim(Str(newval.ms))
      Return _setAttr("move", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Performs a smooth move at constant speed toward a given position.
    ''' <para>
    ''' </para>
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <param name="target">
    '''   new position at the end of the move
    ''' </param>
    ''' <param name="ms_duration">
    '''   total duration of the move, in milliseconds
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
    Public Function move(ByVal target As Integer,ByVal ms_duration As Integer) As Integer
      Dim rest_val As String
      rest_val = Ltrim(Str(target))+":"+Ltrim(Str(ms_duration))
      Return _setAttr("move", rest_val)
    End Function

    '''*
    ''' <summary>
    '''   Continues the enumeration of servos started using <c>yFirstServo()</c>.
    ''' <para>
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YServo</c> object, corresponding to
    '''   a servo currently online, or a <c>null</c> pointer
    '''   if there are no more servos to enumerate.
    ''' </returns>
    '''/
    Public Function nextServo() as YServo
      Dim hwid As String =""
      If (YISERR(_nextFunction(hwid))) Then
        Return Nothing
      End If
      If (hwid="") Then
        Return Nothing
      End If
      Return yFindServo(hwid)
    End Function

    '''*
    ''' <summary>
    '''   comment from .
    ''' <para>
    '''   yc definition
    ''' </para>
    ''' </summary>
    '''/
  Public Overloads Sub registerValueCallback(ByVal callback As UpdateCallback)
   If (callback IsNot Nothing) Then
     registerFuncCallback(Me)
   Else
     unregisterFuncCallback(Me)
   End If
   _callback = callback
  End Sub

  Public Sub set_callback(ByVal callback As UpdateCallback)
    registerValueCallback(callback)
  End Sub

  Public Sub setCallback(ByVal callback As UpdateCallback)
    registerValueCallback(callback)
  End Sub

  Public Overrides Sub advertiseValue(ByVal value As String)
    If (_callback IsNot Nothing) Then _callback(Me, value)
  End Sub


    '''*
    ''' <summary>
    '''   Retrieves a servo for a given identifier.
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
    '''   This function does not require that the servo is online at the time
    '''   it is invoked. The returned object is nevertheless valid.
    '''   Use the method <c>YServo.isOnline()</c> to test if the servo is
    '''   indeed online at a given time. In case of ambiguity when looking for
    '''   a servo by logical name, no error is notified: the first instance
    '''   found is returned. The search is performed first by hardware name,
    '''   then by logical name.
    ''' </para>
    ''' </summary>
    ''' <param name="func">
    '''   a string that uniquely characterizes the servo
    ''' </param>
    ''' <returns>
    '''   a <c>YServo</c> object allowing you to drive the servo.
    ''' </returns>
    '''/
    Public Shared Function FindServo(ByVal func As String) As YServo
      Dim res As YServo
      If (_ServoCache.ContainsKey(func)) Then
        Return CType(_ServoCache(func), YServo)
      End If
      res = New YServo(func)
      _ServoCache.Add(func, res)
      Return res
    End Function

    '''*
    ''' <summary>
    '''   Starts the enumeration of servos currently accessible.
    ''' <para>
    '''   Use the method <c>YServo.nextServo()</c> to iterate on
    '''   next servos.
    ''' </para>
    ''' </summary>
    ''' <returns>
    '''   a pointer to a <c>YServo</c> object, corresponding to
    '''   the first servo currently online, or a <c>null</c> pointer
    '''   if there are none.
    ''' </returns>
    '''/
    Public Shared Function FirstServo() As YServo
      Dim v_fundescr(1) As YFUN_DESCR
      Dim dev As YDEV_DESCR
      Dim neededsize, err As Integer
      Dim serial, funcId, funcName, funcVal As String
      Dim errmsg As String = ""
      Dim size As Integer = Marshal.SizeOf(v_fundescr(0))
      Dim p As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr(0)))

      err = yapiGetFunctionsByClass("Servo", 0, p, size, neededsize, errmsg)
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
      Return YServo.FindServo(serial + "." + funcId)
    End Function

    REM --- (end of YServo implementation)

  End Class

  REM --- (Servo functions)

  '''*
  ''' <summary>
  '''   Retrieves a servo for a given identifier.
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
  '''   This function does not require that the servo is online at the time
  '''   it is invoked. The returned object is nevertheless valid.
  '''   Use the method <c>YServo.isOnline()</c> to test if the servo is
  '''   indeed online at a given time. In case of ambiguity when looking for
  '''   a servo by logical name, no error is notified: the first instance
  '''   found is returned. The search is performed first by hardware name,
  '''   then by logical name.
  ''' </para>
  ''' </summary>
  ''' <param name="func">
  '''   a string that uniquely characterizes the servo
  ''' </param>
  ''' <returns>
  '''   a <c>YServo</c> object allowing you to drive the servo.
  ''' </returns>
  '''/
  Public Function yFindServo(ByVal func As String) As YServo
    Return YServo.FindServo(func)
  End Function

  '''*
  ''' <summary>
  '''   Starts the enumeration of servos currently accessible.
  ''' <para>
  '''   Use the method <c>YServo.nextServo()</c> to iterate on
  '''   next servos.
  ''' </para>
  ''' </summary>
  ''' <returns>
  '''   a pointer to a <c>YServo</c> object, corresponding to
  '''   the first servo currently online, or a <c>null</c> pointer
  '''   if there are none.
  ''' </returns>
  '''/
  Public Function yFirstServo() As YServo
    Return YServo.FirstServo()
  End Function

  Private Sub _ServoCleanup()
  End Sub


  REM --- (end of Servo functions)

End Module
