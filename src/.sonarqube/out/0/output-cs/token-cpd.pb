¢
YC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\TraceEvent.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

enum 

TraceEvent 
{ 
Request 
, 
Response 
, 
Error 
, 
Information 
}		 
}

 ¸
TC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Stuff.cs
[ 
assembly 	
:	 

InternalsVisibleTo 
( 
$str .
,. /
AllInternalsVisible0 C
=D E
trueF J
)J K
]K L
[ 
assembly 	
:	 

InternalsVisibleTo 
( 
$str 8
,8 9
AllInternalsVisible: M
=N O
trueP T
)T U
]U V
[ 
assembly 	
:	 

InternalsVisibleTo 
( 
$str B
,B C
AllInternalsVisibleD W
=X Y
trueZ ^
)^ _
]_ `Ü
fC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\ResponseOfTResponseBody.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class 
Response 
< 
TResponseBody '
>' (
:) *
Response+ 3
{ 
public		 
TResponseBody		 
?		 
Body		 "
{		# $
get		% (
;		( )
}		* +
public 
Response 
( 
IHeadersCollection 
headersCollection ,
,, -
int 

statusCode 
, 
HttpRequestMethod 
httpRequestMethod +
,+ ,
byte 
[ 
] 
responseData 
, 
TResponseBody 
? 
body 
, 
AbsoluteUrl 

requestUri 
) 	
:
 
base 
( 
headersCollection 
, 

statusCode 
, 
httpRequestMethod 
, 
responseData 
, 

requestUri 
) 
=> 
Body 
=  !
body" &
;& '
public 
static 
implicit 
operator '
TResponseBody( 5
?5 6
(6 7
Response7 ?
<? @
TResponseBody@ M
>M N
responseO W
)W X
=> 
response 
!= 
null 
&&  "
response# +
.+ ,
Body, 0
!=1 3
null4 8
?9 :
response; C
.C D
BodyD H
:I J
defaultK R
;R S
} 
} Ì
WC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Response.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

abstract 
class 
Response "
:# $
	IResponse% .
{ 
private		 
readonly		 
byte		 
[		 
]		 
responseData		  ,
;		, -
public 
bool 
	IsSuccess 
=>  

StatusCode! +
is, .
>=/ 1
$num2 5
and6 9
<=: <
$num= @
;@ A
public 
virtual 
int 

StatusCode %
{& '
get( +
;+ ,
}- .
public 
virtual 
IHeadersCollection )
Headers* 1
{2 3
get4 7
;7 8
}9 :
public 
virtual 
HttpRequestMethod (
HttpRequestMethod) :
{; <
get= @
;@ A
}B C
public 
virtual 
AbsoluteUrl "

RequestUri# -
{. /
get0 3
;3 4
}5 6
	protected 
Response 
( 	
IHeadersCollection 
headersCollection ,
,, -
int 

statusCode 
, 
HttpRequestMethod 
httpRequestMethod +
,+ ,
byte 
[ 
] 
responseData 
, 
AbsoluteUrl 

requestUri 
) 	
{ 	

StatusCode 
= 

statusCode #
;# $
Headers 
= 
headersCollection '
;' (
HttpRequestMethod   
=   
httpRequestMethod    1
;  1 2

RequestUri!! 
=!! 

requestUri!! #
;!!# $
this"" 
."" 
responseData"" 
="" 
responseData""  ,
;"", -
}## 	
public'' 
virtual'' 
byte'' 
['' 
]'' 
GetResponseData'' -
(''- .
)''. /
=>''0 2
responseData''3 ?
;''? @
})) 
}** ∆
VC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Request.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class 
Request 
< 
TBody 
> 
:  !
IRequest" *
<* +
TBody+ 0
>0 1
{ 
public 
TBody 
? 
BodyData 
{  
get! $
;$ %
}& '
public 
IHeadersCollection !
Headers" )
{* +
get, /
;/ 0
}1 2
public 
AbsoluteUrl 
Uri 
{  
get! $
;$ %
}& '
public 
HttpRequestMethod  
HttpRequestMethod! 2
{3 4
get5 8
;8 9
}: ;
public 
CancellationToken  
CancellationToken! 2
{3 4
get5 8
;8 9
}: ;
public 
string 
? #
CustomHttpRequestMethod .
{/ 0
get1 4
;4 5
}6 7
public 
Request 
( 
AbsoluteUrl 
uri 
, 
TBody   
?   
bodyData   
,   
IHeadersCollection!! 
headers!! &
,!!& '
HttpRequestMethod"" 
httpRequestMethod"" /
,""/ 0
string## 
?## #
customHttpRequestMethod## +
=##, -
null##. 2
,##2 3
CancellationToken$$ 
cancellationToken$$ /
=$$0 1
default$$2 9
)$$9 :
{%% 	
BodyData&& 
=&& 
bodyData&& 
;&&  
Uri'' 
='' 
uri'' 
;'' 
HttpRequestMethod(( 
=(( 
httpRequestMethod((  1
;((1 2
CancellationToken)) 
=)) 
cancellationToken))  1
;))1 2#
CustomHttpRequestMethod** #
=**$ %#
customHttpRequestMethod**& =
;**= >
Headers++ 
=++ 
headers++ 
;++ 
if-- 
(-- 
uri-- 
==-- 
null-- 
)-- 
throw-- "
new--# &!
ArgumentNullException--' <
(--< =
nameof--= C
(--C D
uri--D G
)--G H
)--H I
;--I J
}.. 	
public00 
override00 
string00 
ToString00 '
(00' (
)00( )
=>00* ,
$"00- /
$str00/ =
{00= >
Uri00> A
}00A B
$str00B O
{00O P
Headers00P W
}00W X
$str00X a
{00a b
HttpRequestMethod00b s
}00s t
"00t u
;00u v
}33 
}44 ˇ

`C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\NullKvpEnumerator.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

class		 
NullKvpEnumerator		 "
<		" #
T		# $
,		$ %
T2		& (
>		( )
:		* +
IEnumerator		, 7
<		7 8
KeyValuePair		8 D
<		D E
T		E F
,		F G
T2		H J
>		J K
>		K L
{

 
public 
KeyValuePair 
< 
T 
, 
T2 !
>! "
Current# *
=>+ -
new. 1
(1 2
)2 3
;3 4
object 
IEnumerator 
. 
Current "
=># %
new& )
KeyValuePair* 6
<6 7
T7 8
,8 9
T2: <
>< =
(= >
)> ?
;? @
public 
void 
Dispose 
( 
) 
{ 
}  !
public 
bool 
MoveNext 
( 
) 
=> !
false" '
;' (
public 
void 
Reset 
( 
) 
{ 
} 
} 
} ·
dC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\NullHeadersCollection.cs
	namespace 	

RestClient
 
. 
Net 
{		 
public

 

class

 !
NullHeadersCollection

 &
:

' (
IHeadersCollection

) ;
,

; <
IDisposable

= H
{ 
private 
readonly 
NullKvpEnumerator *
<* +
string+ 1
,1 2
IEnumerable3 >
<> ?
string? E
>E F
>F G
nullEnumeratorH V
=W X
newY \
(\ ]
)] ^
;^ _
public 
static !
NullHeadersCollection +
Instance, 4
{5 6
get7 :
;: ;
}< =
=> ?
new@ C
(C D
)D E
;E F
public 
IEnumerable 
< 
string !
>! "
Names# (
{) *
get+ .
;. /
}0 1
=2 3
new4 7
List8 <
<< =
string= C
>C D
(D E
)E F
;F G
public 
IEnumerable 
< 
string !
>! "
this# '
[' (
string( .
name/ 3
]3 4
=>5 7
new8 ;
List< @
<@ A
stringA G
>G H
(H I
)I J
;J K
public 
bool 
Contains 
( 
string #
name$ (
)( )
=>* ,
false- 2
;2 3
public 
IEnumerator 
< 
KeyValuePair '
<' (
string( .
,. /
IEnumerable0 ;
<; <
string< B
>B C
>C D
>D E
GetEnumeratorF S
(S T
)T U
=>V X
nullEnumeratorY g
;g h
IEnumerator 
IEnumerable 
.  
GetEnumerator  -
(- .
). /
=>0 2
nullEnumerator3 A
;A B
public 
void 
Dispose 
( 
) 
=>  
nullEnumerator! /
./ 0
Dispose0 7
(7 8
)8 9
;9 :
} 
}   ¨
eC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\MissingHeaderException.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class "
MissingHeaderException '
:( )
	Exception* 3
{ 
public 
bool 
	IsRequest 
{ 
get  #
;# $
}% &
public		 "
MissingHeaderException		 %
(		% &
string

 
message

 
,

 
bool 
	isRequest 
, 
	Exception 
innerException $
)$ %
:& '
base 
( 
message 
, 
innerException (
)( )
=>* ,
	IsRequest- 6
=7 8
	isRequest9 B
;B C
} 
} ú
_C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\LogLevel.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
{ 
public 

enum 
LogLevel 
{ 
Trace 
= 
$num 
, 
Debug 
= 
$num 
, 
Information 
= 
$num 
, 
Warning 
= 
$num 
, 
Error"" 
="" 
$num"" 
,"" 
Critical'' 
='' 
$num'' 
,'' 
None,, 
=,, 
$num,, 
}-- 
}.. ƒ
WC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Messages.cs
	namespace 	

RestClient
 
. 
Net 
{ 
internal 
static 
class 
Messages "
{ 
public 
const 
string '
InfoSendReturnedNoException 7
=8 9
$str: q
;q r
public 
const 
string +
ErrorMessageAbsoluteUrlAsString ;
=< =
$str	> Ø
;
Ø ∞
public		 
const		 
string		 '
ErrorMessageDeserialization		 7
=		8 9
$str			: ò
;
		ò ô
public

 
const

 
string

 +
ErrorMessageHeaderAlreadyExists

 ;
=

< =
$str

> l
;

l m
public 
const 
string  
InfoAttemptingToSend 0
=1 2
$str3 f
;f g
public 
const 
string "
TraceResponseProcessed 2
=3 4
$str5 d
;d e
public 
const 
string 
TraceBeginSend *
=+ ,
$str	- ß
;
ß ®
public 
const 
string 
ErrorOnSend '
=( )
$str* I
;I J
public 
const 
string 
ErrorTaskCancelled .
=/ 0
$str1 R
;R S
public 
const 
string 
ErrorSendException .
=/ 0
$str1 L
;L M
public 
const 
string *
ErrorMessageOperationCancelled :
=; <
$str= \
;\ ]
public 
static 
string %
GetErrorMessageNonSuccess 6
(6 7
int7 :
responseCode; G
,G H
AbsoluteUrlI T
?T U

requestUriV `
)` a
=>b d
$"e g
$str	g à
{
à â
responseCode
â ï
}
ï ñ
$str
ñ ®
{
® ©

requestUri
© ≥
}
≥ ¥
"
¥ µ
;
µ ∂
} 
} ∞$
dC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\NullLoggerOfT.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
.& '
Abstractions' 3
{ 
internal 
class 

NullLogger 
< 
T 
>  
:! "
ILogger# *
<* +
T+ ,
>, -
{		 
private

 
readonly

 
ILogger

  
_logger

! (
;

( )
public 
static 

NullLogger  
<  !
T! "
>" #
Instance$ ,
=- .
new/ 2
(2 3
NullLoggerFactory3 D
.D E
InstanceE M
)M N
;N O
public 

NullLogger 
( 
ILoggerFactory (
factory) 0
)0 1
{ 	
if 
( 
factory 
== 
null 
)  
{ 
throw 
new !
ArgumentNullException /
(/ 0
nameof0 6
(6 7
factory7 >
)> ?
)? @
;@ A
} 
_logger 
= 
factory 
. 
CreateLogger *
(* +
typeof+ 1
(1 2
T2 3
)3 4
.4 5
Name5 9
)9 :
;: ;
} 	
public 
IDisposable 

BeginScope %
(% &
string& ,
messageFormat- :
,: ;
params< B
objectC I
[I J
]J K
argsL P
)P Q
=>R T
_loggerU \
.\ ]

BeginScope] g
(g h
messageFormath u
,u v
argsw {
){ |
;| }
public 
void 
LogDebug 
( 
string #
message$ +
,+ ,
params- 3
object4 :
[: ;
]; <
args= A
)A B
=>C E
_loggerF M
.M N
LogDebugN V
(V W
messageW ^
,^ _
args` d
)d e
;e f
public 
void 
LogError 
( 
EventId $
eventId% ,
,, -
	Exception. 7
	exception8 A
,A B
stringC I
messageJ Q
,Q R
paramsS Y
objectZ `
[` a
]a b
argsc g
)g h
=>i k
_loggerl s
.s t
LogErrort |
(| }
eventId	} Ñ
,
Ñ Ö
	exception
Ü è
,
è ê
message
ë ò
,
ò ô
args
ö û
)
û ü
;
ü †
public 
void 
LogError 
( 
	Exception &
	exception' 0
,0 1
string2 8
message9 @
,@ A
paramsB H
objectI O
[O P
]P Q
argsR V
)V W
=>X Z
_logger[ b
.b c
LogErrorc k
(k l
	exceptionl u
,u v
messagew ~
,~ 
args
Ä Ñ
)
Ñ Ö
;
Ö Ü
public 
void 
LogInformation "
(" #
string# )
message* 1
,1 2
params3 9
object: @
[@ A
]A B
argsC G
)G H
=>I K
_loggerL S
.S T
LogInformationT b
(b c
messagec j
,j k
argsl p
)p q
;q r
public 
void 
LogTrace 
( 
string #
message$ +
,+ ,
params- 3
object4 :
[: ;
]; <
args= A
)A B
{C D
}E F
public 
void 

LogWarning 
( 
string %
message& -
,- .
params/ 5
object6 <
[< =
]= >
args? C
)C D
=>E G
_loggerH O
.O P

LogWarningP Z
(Z [
message[ b
,b c
argsd h
)h i
;i j
} 
}   ı
hC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\NullLoggerFactory.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
.& '
Abstractions' 3
{ 
public 

class 
NullLoggerFactory "
:# $
ILoggerFactory% 3
{ 
public		 
static		 
NullLoggerFactory		 '
Instance		( 0
{		1 2
get		3 6
;		6 7
}		8 9
=		: ;
new		< ?
(		? @
)		@ A
;		A B
public 
ILogger 
CreateLogger #
(# $
string$ *
name+ /
)/ 0
=>1 3

NullLogger4 >
.> ?
Instance? G
;G H
public 
void 
Dispose 
( 
) 
{ 
}  !
} 
} Œ
aC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\NullLogger.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
.& '
Abstractions' 3
{ 
public 

class 

NullLogger 
: 
ILogger %
{		 
public

 
static

 

NullLogger

  
Instance

! )
{

* +
get

, /
;

/ 0
}

1 2
=

3 4
new

5 8
(

8 9
)

9 :
;

: ;
public 
IDisposable 

BeginScope %
(% &
string& ,
messageFormat- :
,: ;
params< B
objectC I
[I J
]J K
argsL P
)P Q
=>R T
newU X
DummyDisposableY h
(h i
)i j
;j k
public 
void 
LogDebug 
( 
string #
message$ +
,+ ,
params- 3
object4 :
[: ;
]; <
args= A
)A B
{ 	
} 	
public 
void 
LogError 
( 
EventId $
eventId% ,
,, -
	Exception. 7
	exception8 A
,A B
stringC I
messageJ Q
,Q R
paramsS Y
objectZ `
[` a
]a b
argsc g
)g h
{ 	
} 	
public 
void 
LogError 
( 
	Exception &
	exception' 0
,0 1
string2 8
message9 @
,@ A
paramsB H
objectI O
[O P
]P Q
argsR V
)V W
{ 	
} 	
public 
void 
LogInformation "
(" #
string# )
message* 1
,1 2
params3 9
object: @
[@ A
]A B
argsC G
)G H
{ 	
} 	
public 
void 
LogTrace 
( 
string #
message$ +
,+ ,
params- 3
object4 :
[: ;
]; <
args= A
)A B
{C D
}E F
public   
void   

LogWarning   
(   
string   %
message  & -
,  - .
params  / 5
object  6 <
[  < =
]  = >
args  ? C
)  C D
{!! 	
}"" 	
}## 
}$$ Ó
nC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\LoggerFactoryExtensions.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
.& '
Abstractions' 3
{ 
public 

static 
class #
LoggerFactoryExtensions /
{ 
public 
static 
ILogger 
< 
T 
>  
CreateLogger! -
<- .
T. /
>/ 0
(0 1
this1 5
ILoggerFactory6 D
loggerFactoryE R
)R S
=>T V
newW Z

NullLogger[ e
<e f
Tf g
>g h
(h i
loggerFactoryi v
)v w
;w x
} 
}		 ã
^C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\ILogger.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
{ 
public 

	interface 
ILogger 
{ 
IDisposable		 

BeginScope		 
(		 
string		 %
messageFormat		& 3
,		3 4
params		5 ;
object		< B
[		B C
]		C D
args		E I
)		I J
;		J K
void

 
LogError

 
(

 
EventId

 
eventId

 %
,

% &
	Exception

' 0
	exception

1 :
,

: ;
string

< B
message

C J
,

J K
params

L R
object

S Y
[

Y Z
]

Z [
args

\ `
)

` a
;

a b
void 
LogError 
( 
	Exception 
	exception  )
,) *
string+ 1
message2 9
,9 :
params; A
objectB H
[H I
]I J
argsK O
)O P
;P Q
void 
LogInformation 
( 
string "
message# *
,* +
params, 2
object3 9
[9 :
]: ;
args< @
)@ A
;A B
void 

LogWarning 
( 
string 
message &
,& '
params( .
object/ 5
[5 6
]6 7
args8 <
)< =
;= >
void 
LogDebug 
( 
string 
message $
,$ %
params& ,
object- 3
[3 4
]4 5
args6 :
): ;
;; <
void 
LogTrace 
( 
string 
message $
,$ %
params& ,
object- 3
[3 4
]4 5
args6 :
): ;
;; <
} 
public 

	interface 
ILogger 
< 
T 
> 
:  !
ILogger" )
{ 
} 
} á
eC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\ILoggerFactory.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
{ 
public 

	interface 
ILoggerFactory #
:$ %
IDisposable& 1
{ 
ILogger		 
CreateLogger		 
(		 
string		 #
name		$ (
)		( )
;		) *
}

 
} «
^C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\EventId.cs
	namespace 	
	Microsoft
 
. 

Extensions 
. 
Logging &
{ 
public 

readonly 
struct 
EventId "
{ 
public 
EventId 
( 
int 
id 
, 
string %
?% &
name' +
=, -
null. 2
)2 3
{		 	
Id

 
=

 
id

 
;

 
Name 
= 
name 
; 
} 	
public 
int 
Id 
{ 
get 
; 
} 
public 
string 
? 
Name 
{ 
get !
;! "
}# $
} 
} “
fC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\Logging\DummyDisposable.cs
	namespace 	

RestClient
 
. 
Net 
{ 
internal 
class 
DummyDisposable "
:# $
IDisposable% 0
{ 
public 
void 
Dispose 
( 
) 
{

 	
} 	
} 
} ú
dC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\ISerializationAdapter.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface !
ISerializationAdapter *
{ 
byte 
[ 
] 
	Serialize 
< 
TRequestBody %
>% &
(& '
TRequestBody' 3
value4 9
,9 :
IHeadersCollection; M
requestHeadersN \
)\ ]
;] ^
TResponseBody 
? 
Deserialize "
<" #
TResponseBody# 0
>0 1
(1 2
byte2 6
[6 7
]7 8
responseData9 E
,E F
IHeadersCollectionG Y
responseHeadersZ i
)i j
;j k
} 
} ú
fC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\ISendHttpRequestMessage.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface #
ISendHttpRequestMessage ,
{ 
Task		 
<		 
HttpResponseMessage		  
>		  !"
SendHttpRequestMessage		" 8
<		8 9
TRequestBody		9 E
>		E F
(		F G

HttpClient

 

httpClient

 !
,

! ""
IGetHttpRequestMessage ""
httpRequestMessageFunc# 9
,9 :
IRequest 
< 
TRequestBody !
>! "
request# *
,* +
ILogger 
logger 
, !
ISerializationAdapter ! 
serializationAdapter" 6
)6 7
;7 8
} 
} Ò	
XC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\IResponse.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface 
	IResponse 
{ 
IHeadersCollection 
Headers "
{# $
get% (
;( )
}* +
HttpRequestMethod 
HttpRequestMethod +
{, -
get. 1
;1 2
}3 4
bool		 
	IsSuccess		 
{		 
get		 
;		 
}		 
AbsoluteUrl

 

RequestUri

 
{

  
get

! $
;

$ %
}

& '
int 

StatusCode 
{ 
get 
; 
} 
byte 
[ 
] 
GetResponseData 
( 
)  
;  !
} 
public 

	interface 
	IResponse 
< 
TResponseBody ,
>, -
:. /
	IResponse0 9
{ 
TResponseBody 
Body 
{ 
get  
;  !
}" #
} 
} £	
WC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\IRequest.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface 
IRequest 
{ 
CancellationToken 
CancellationToken +
{, -
get. 1
;1 2
}3 4
string		 
?		 #
CustomHttpRequestMethod		 '
{		( )
get		* -
;		- .
}		/ 0
IHeadersCollection

 
Headers

 "
{

# $
get

% (
;

( )
}

* +
HttpRequestMethod 
HttpRequestMethod +
{, -
get. 1
;1 2
}3 4
AbsoluteUrl 
Uri 
{ 
get 
; 
}  
} 
public 

	interface 
IRequest 
< 
TBody #
># $
:% &
IRequest' /
{ 
TBody 
? 
BodyData 
{ 
get 
; 
}  
} 
} ï
aC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\IHeadersCollection.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface 
IHeadersCollection '
:( )
IEnumerable* 5
<5 6
KeyValuePair6 B
<B C
stringC I
,I J
IEnumerableK V
<V W
stringW ]
>] ^
>^ _
>_ `
{		 
IEnumerable 
< 
string 
> 
this  
[  !
string! '
name( ,
], -
{ 	
get 
; 
} 	
bool 
Contains 
( 
string 
name !
)! "
;" #
IEnumerable 
< 
string 
> 
Names !
{" #
get$ '
;' (
}) *
} 
}   ¡
eC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\IGetHttpRequestMessage.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

	interface "
IGetHttpRequestMessage +
{ 
HttpRequestMessage !
GetHttpRequestMessage 0
<0 1
T1 2
>2 3
(3 4
IRequest4 <
<< =
T= >
>> ?
request@ G
,G H
ILoggerI P
loggerQ W
,W X!
ISerializationAdapterY n!
serializationAdapter	o É
)
É Ñ
;
Ñ Ö
}		 
}

 í
VC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\IClient.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

	interface		 
IClient		 
{

 
Task 
< 
Response 
< 
TResponseBody #
># $
>$ %
	SendAsync& /
</ 0
TResponseBody0 =
,= >
TRequestBody? K
>K L
(L M
IRequestM U
<U V
TRequestBodyV b
>b c
requestd k
)k l
;l m
IHeadersCollection !
DefaultRequestHeaders 0
{1 2
get3 6
;6 7
}8 9
AbsoluteUrl 
BaseUrl 
{ 
get !
;! "
}# $
} 
} º
bC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\HttpStatusException.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class 
HttpStatusException $
:% &
	Exception' 0
{ 
public 
	IResponse 
Response !
{" #
get$ '
;' (
}) *
public		 
HttpStatusException		 "
(		" #
string

 
message

 
,

 
	IResponse 
response 
) 
:  !
base" &
(& '
message' .
). /
=>0 2
Response3 ;
=< =
response> F
;F G
} 
} Ê
`C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\HttpRequestMethod.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

enum 
HttpRequestMethod !
{ 
Post 
, 
Get 
, 
Put 
, 
Delete 
, 
Patch		 
,		 
Custom

 
} 
} ©#
`C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\HeadersCollection.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

sealed 
class 
HeadersCollection )
:* +
IHeadersCollection, >
{		 
private 
readonly 
IDictionary $
<$ %
string% +
,+ ,
IEnumerable- 8
<8 9
string9 ?
>? @
>@ A

dictionaryB L
;L M
public 
HeadersCollection  
(  !
IDictionary! ,
<, -
string- 3
,3 4
IEnumerable5 @
<@ A
stringA G
>G H
>H I

dictionaryJ T
)T U
=>V X
thisY ]
.] ^

dictionary^ h
=i j

dictionaryk u
;u v
public 
HeadersCollection  
(  !
string! '
key( +
,+ ,
string- 3
value4 9
)9 :
:; <
this= A
(A B
ImmutableDictionaryB U
.U V
CreateRangeV a
(a b
new 
List 
< 
KeyValuePair )
<) *
string* 0
,0 1
IEnumerable2 =
<= >
string> D
>D E
>E F
>F G
{ 
new 
( 
key 
,  
ImmutableList! .
.. /
Create/ 5
(5 6
value6 ;
); <
)< =
} 
) 
) 
{ 	
} 	
public 
static 
HeadersCollection '
Empty( -
{. /
get0 3
;3 4
}5 6
=7 8
new9 <
HeadersCollection= N
(N O
ImmutableDictionaryO b
.b c
Createc i
<i j
stringj p
,p q
IEnumerabler }
<} ~
string	~ Ñ
>
Ñ Ö
>
Ö Ü
(
Ü á
)
á à
)
à â
;
â ä
public 
IEnumerable 
< 
string !
>! "
Names# (
=>) +

dictionary, 6
.6 7
Keys7 ;
;; <
IEnumerable   
<   
string   
>   
IHeadersCollection   .
.  . /
this  / 3
[  3 4
string  4 :
name  ; ?
]  ? @
=>  A C

dictionary  D N
[  N O
name  O S
]  S T
;  T U
public$$ 
bool$$ 
Contains$$ 
($$ 
string$$ #
name$$$ (
)$$( )
=>$$* ,

dictionary$$- 7
.$$7 8
ContainsKey$$8 C
($$C D
name$$D H
)$$H I
;$$I J
public&& 
IEnumerator&& 
<&& 
KeyValuePair&& '
<&&' (
string&&( .
,&&. /
IEnumerable&&0 ;
<&&; <
string&&< B
>&&B C
>&&C D
>&&D E
GetEnumerator&&F S
(&&S T
)&&T U
=>&&V X

dictionary&&Y c
.&&c d
GetEnumerator&&d q
(&&q r
)&&r s
;&&s t
IEnumerator(( 
IEnumerable(( 
.((  
GetEnumerator((  -
(((- .
)((. /
=>((0 2

dictionary((3 =
.((= >
GetEnumerator((> K
(((K L
)((L M
;((M N
public)) 
override)) 
string)) 
ToString)) '
())' (
)))( )
=>))* ,
string))- 3
.))3 4
Join))4 8
())8 9
$str))9 ?
,))? @

dictionary))A K
.))K L
Select))L R
())R S
kvp))S V
=>))W Y
$"))Z \
{))\ ]
kvp))] `
.))` a
Key))a d
}))d e
$str))e g
{))g h
string))h n
.))n o
Join))o s
())s t
$str))t x
,))x y
kvp))z }
.))} ~
Value	))~ É
)
))É Ñ
}
))Ñ Ö
"
))Ö Ü
)
))Ü á
)
))á à
;
))à â
}++ 
},, æè
`C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\HeadersExtensions.cs
	namespace 	

RestClient
 
. 
Net 
{		 
public

 

static

 
class

 
HeadersExtensions

 )
{ 
internal 
const 
string 
Authorization +
=, -
$str. =
;= >
public 
const 
string !
ContentTypeHeaderName 1
=2 3
$str4 B
;B C
public 
const 
string %
ContentEncodingHeaderName 5
=6 7
$str8 J
;J K
public 
const 
string %
ContentLanguageHeaderName 5
=6 7
$str8 J
;J K
public 
const 
string #
ContentLengthHeaderName 3
=4 5
$str6 F
;F G
public 
const 
string %
ContentLocationHeaderName 5
=6 7
$str8 J
;J K
public 
const 
string  
ContentMD5HeaderName 0
=1 2
$str3 @
;@ A
public 
const 
string "
ContentRangeHeaderName 2
=3 4
$str5 D
;D E
internal 
static 
List 
< 
string #
># $
ContentHeaderNames% 7
=8 9
new: =
(= >
)> ?
{ 	!
ContentTypeHeaderName !
,! "%
ContentEncodingHeaderName %
,% &%
ContentLanguageHeaderName   %
,  % &#
ContentLengthHeaderName!! #
,!!# $%
ContentLocationHeaderName"" %
,""% & 
ContentMD5HeaderName##  
,##  !"
ContentRangeHeaderName$$ "
}%% 	
;%%	 

internal'' 
const'' 
string'' 
JsonMediaType'' +
='', -
$str''. @
;''@ A
internal(( 
const(( 
string(( 
ContentEncodingGzip(( 1
=((2 3
$str((4 :
;((: ;
internal)) 
const)) 
string)) 
ContentLanguage)) -
=)). /
$str))0 6
;))6 7
public// 
static// 
IHeadersCollection// (
Append//) /
(/// 0
this//0 4
IHeadersCollection//5 G
headersCollection//H Y
,//Y Z
string//[ a
key//b e
,//e f
IEnumerable//g r
<//r s
string//s y
>//y z
value	//{ Ä
)
//Ä Å
{00 	
if11 
(11 
headersCollection11 !
==11" $
null11% )
)11) *
throw11+ 0
new111 4!
ArgumentNullException115 J
(11J K
nameof11K Q
(11Q R
headersCollection11R c
)11c d
)11d e
;11e f
if22 
(22 
key22 
==22 
null22 
)22 
throw22 "
new22# &!
ArgumentNullException22' <
(22< =
nameof22= C
(22C D
key22D G
)22G H
)22H I
;22I J
if33 
(33 
value33 
==33 
null33 
)33 
throw33 $
new33% (!
ArgumentNullException33) >
(33> ?
nameof33? E
(33E F
value33F K
)33K L
)33L M
;33M N
var55 

dictionary55 
=55 
headersCollection55 .
.55. /
ToDictionary55/ ;
(55; <
kvp55< ?
=>55@ B
kvp55C F
.55F G
Key55G J
,55J K
kvp55L O
=>55P R
kvp55S V
.55V W
Value55W \
)55\ ]
;55] ^

dictionary77 
.77 
Add77 
(77 
key77 
,77 
value77  %
)77% &
;77& '
return99 
new99 
HeadersCollection99 (
(99( )

dictionary99) 3
)993 4
;994 5
}:: 	
public<< 
static<< 
IHeadersCollection<< (
Append<<) /
(<</ 0
this<<0 4
IHeadersCollection<<5 G
headersCollection<<H Y
,<<Y Z
string<<[ a
key<<b e
,<<e f
string<<g m
value<<n s
)<<s t
=>== 

Append== 
(== 
headersCollection== #
,==# $
key==% (
,==( )
new==* -
List==. 2
<==2 3
string==3 9
>==9 :
{==; <
value=== B
}==C D
)==D E
;==E F
public?? 
static?? 
IHeadersCollection?? (
Append??) /
(??/ 0
this??0 4
IHeadersCollection??5 G
???G H
headersCollection??I Z
,??Z [
IHeadersCollection??\ n
???n o
headersCollection2	??p Ç
)
??Ç É
{@@ 	
varBB 

dictionaryBB 
=BB 
newBB  

DictionaryBB! +
<BB+ ,
stringBB, 2
,BB2 3
IEnumerableBB4 ?
<BB? @
stringBB@ F
>BBF G
>BBG H
(BBH I
)BBI J
;BBJ K
ifDD 
(DD 
headersCollectionDD !
!=DD" $
nullDD% )
)DD) *
{EE 
foreachFF 
(FF 
varFF 
kvpFF  
inFF! #
headersCollectionFF$ 5
)FF5 6
{GG 

dictionaryHH 
.HH 
AddHH "
(HH" #
kvpHH# &
.HH& '
KeyHH' *
,HH* +
kvpHH, /
.HH/ 0
ValueHH0 5
)HH5 6
;HH6 7
}II 
}JJ 
ifLL 
(LL 
headersCollection2LL "
==LL# %
nullLL& *
)LL* +
returnLL, 2
newLL3 6
HeadersCollectionLL7 H
(LLH I

dictionaryLLI S
)LLS T
;LLT U
foreachNN 
(NN 
varNN 
kvpNN 
inNN 
headersCollection2NN  2
)NN2 3
{OO 
ifPP 
(PP 

dictionaryPP 
.PP 
ContainsKeyPP *
(PP* +
kvpPP+ .
.PP. /
KeyPP/ 2
)PP2 3
)PP3 4
_PP5 6
=PP7 8

dictionaryPP9 C
.PPC D
RemovePPD J
(PPJ K
kvpPPK N
.PPN O
KeyPPO R
)PPR S
;PPS T

dictionaryRR 
.RR 
AddRR 
(RR 
kvpRR "
.RR" #
KeyRR# &
,RR& '
kvpRR( +
.RR+ ,
ValueRR, 1
)RR1 2
;RR2 3
}SS 
returnUU 
newUU 
HeadersCollectionUU (
(UU( )

dictionaryUU) 3
)UU3 4
;UU4 5
}VV 	
publicXX 
staticXX 
IHeadersCollectionXX ('
AppendDefaultRequestHeadersXX) D
(XXD E
thisXXE I
IClientXXJ Q
clientXXR X
,XXX Y
IHeadersCollectionXXZ l
headersCollectionXXm ~
)XX~ 
{YY 	
ifZZ 
(ZZ 
clientZZ 
==ZZ 
nullZZ 
)ZZ 
throwZZ  %
newZZ& )!
ArgumentNullExceptionZZ* ?
(ZZ? @
nameofZZ@ F
(ZZF G
clientZZG M
)ZZM N
)ZZN O
;ZZO P
if[[ 
([[ 
headersCollection[[ !
==[[" $
null[[% )
)[[) *
throw[[+ 0
new[[1 4!
ArgumentNullException[[5 J
([[J K
nameof[[K Q
([[Q R
headersCollection[[R c
)[[c d
)[[d e
;[[e f
var]] 

dictionary]] 
=]] 
new]]  

Dictionary]]! +
<]]+ ,
string]], 2
,]]2 3
IEnumerable]]4 ?
<]]? @
string]]@ F
>]]F G
>]]G H
(]]H I
)]]I J
;]]J K
if__ 
(__ 
client__ 
.__ !
DefaultRequestHeaders__ ,
!=__- /
null__0 4
)__4 5
{`` 
foreachaa 
(aa 
varaa 
kvpaa  
inaa! #
clientaa$ *
.aa* +!
DefaultRequestHeadersaa+ @
)aa@ A
{bb 

dictionarycc 
.cc 
Addcc "
(cc" #
kvpcc# &
.cc& '
Keycc' *
,cc* +
kvpcc, /
.cc/ 0
Valuecc0 5
)cc5 6
;cc6 7
}dd 
}ee 
foreachgg 
(gg 
vargg 
kvpgg 
ingg 
headersCollectiongg  1
)gg1 2
{hh 
ifii 
(ii 

dictionaryii 
.ii 
ContainsKeyii *
(ii* +
kvpii+ .
.ii. /
Keyii/ 2
)ii2 3
)ii3 4
_ii5 6
=ii7 8

dictionaryii9 C
.iiC D
RemoveiiD J
(iiJ K
kvpiiK N
.iiN O
KeyiiO R
)iiR S
;iiS T

dictionarykk 
.kk 
Addkk 
(kk 
kvpkk "
.kk" #
Keykk# &
,kk& '
kvpkk( +
.kk+ ,
Valuekk, 1
)kk1 2
;kk2 3
}ll 
returnnn 
newnn 
HeadersCollectionnn (
(nn( )

dictionarynn) 3
)nn3 4
;nn4 5
}oo 	
publicqq 
staticqq 
IHeadersCollectionqq (
ToHeadersCollectionqq) <
(qq< =
thisqq= A
stringqqB H
keyqqI L
,qqL M
stringqqN T
valueqqU Z
)qqZ [
=>rr 

newrr 
HeadersCollectionrr  
(rr  !
ImmutableDictionaryrr! 4
.rr4 5
CreateRangerr5 @
(rr@ A
newrrA D
ListrrE I
<rrI J
KeyValuePairrrJ V
<rrV W
stringrrW ]
,rr] ^
IEnumerablerr_ j
<rrj k
stringrrk q
>rrq r
>rrr s
>rrs t
{rru v
newrrw z
(rrz {
keyrr{ ~
,rr~ 
new
rrÄ É
List
rrÑ à
<
rrà â
string
rrâ è
>
rrè ê
{
rrë í
value
rrì ò
}
rrô ö
)
rrö õ
}
rrú ù
)
rrù û
)
rrû ü
;
rrü †
publictt 
statictt 
IHeadersCollectiontt (
ToHeadersCollectiontt) <
(tt< =
thistt= A
KeyValuePairttB N
<ttN O
stringttO U
,ttU V
IEnumerablettW b
<ttb c
stringttc i
>tti j
>ttj k
kvpttl o
)tto p
=>uu 
newuu 
HeadersCollectionuu $
(uu$ %
ImmutableDictionaryuu% 8
.uu8 9
CreateRangeuu9 D
(uuD E
newuuE H
ListuuI M
<uuM N
KeyValuePairuuN Z
<uuZ [
stringuu[ a
,uua b
IEnumerableuuc n
<uun o
stringuuo u
>uuu v
>uuv w
>uuw x
{uuy z
kvpuu{ ~
}	uu Ä
)
uuÄ Å
)
uuÅ Ç
;
uuÇ É
publiczz 
staticzz 
IHeadersCollectionzz ( 
FromBasicCredentialszz) =
(zz= >
stringzz> D
userNamezzE M
,zzM N
stringzzO U
passwordzzV ^
)zz^ _
=>zz` b#
WithBasicAuthenticationzzc z
(zzz {
nullzz{ 
,	zz Ä
userName
zzÅ â
,
zzâ ä
password
zzã ì
)
zzì î
;
zzî ï
public|| 
static|| 
IHeadersCollection|| (
FromBearerToken||) 8
(||8 9
string||9 ?
bearerToken||@ K
)||K L
=>}} 
)
WithBearerTokenAuthentication}} (
(}}( )
null}}) -
,}}- .
bearerToken}}/ :
)}}: ;
;}}; <
public 
static 
IHeadersCollection (
FromJsonContentType) <
(< =
)= >
=>
ÄÄ 
'
WithJsonContentTypeHeader
ÄÄ $
(
ÄÄ$ %
null
ÄÄ% )
)
ÄÄ) *
;
ÄÄ* +
public
ÇÇ 
static
ÇÇ  
IHeadersCollection
ÇÇ (
WithHeaderValue
ÇÇ) 8
(
ÇÇ8 9
this
ÉÉ  
IHeadersCollection
ÉÉ #
?
ÉÉ# $
requestHeaders
ÉÉ% 3
,
ÉÉ3 4
string
ÑÑ 
key
ÑÑ 
,
ÑÑ 
string
ÖÖ 
value
ÖÖ 
)
ÖÖ 
=>
ÜÜ 
_
áá 
=
áá 
requestHeaders
áá 
==
áá !
null
áá" &
?
áá' (
new
àà 
HeadersCollection
àà !
(
àà! "!
ImmutableDictionary
ââ #
.
ââ# $
CreateRange
ââ$ /
(
ââ/ 0
new
ää 
List
ää 
<
ää 
KeyValuePair
ää )
<
ää) *
string
ää* 0
,
ää0 1
IEnumerable
ää2 =
<
ää= >
string
ää> D
>
ääD E
>
ääE F
>
ääF G
{
ãã 
new
åå 
(
åå 
key
åå 
,
åå  
ImmutableList
åå! .
.
åå. /
Create
åå/ 5
(
åå5 6
value
åå6 ;
)
åå; <
)
åå< =
}
çç 
)
éé 
)
éé 
:
éé 
requestHeaders
èè 
.
èè 
Append
èè !
(
èè! "
key
èè" %
,
èè% &
value
èè' ,
)
èè, -
;
èè- .
public
ëë 
static
ëë  
IHeadersCollection
ëë (!
ToHeadersCollection
ëë) <
(
ëë< =
this
ëë= A!
HttpResponseHeaders
ëëB U
?
ëëU V!
httpResponseHeaders
ëëW j
)
ëëj k
=>
íí 
new
íí 
HeadersCollection
íí $
(
íí$ %!
httpResponseHeaders
íí% 8
==
íí9 ;
null
íí< @
?
ííA B!
ImmutableDictionary
ííC V
.
ííV W
Create
ííW ]
<
íí] ^
string
íí^ d
,
ííd e
IEnumerable
ííf q
<
ííq r
string
íír x
>
ííx y
>
ííy z
(
ííz {
)
íí{ |
:
íí} ~"
httpResponseHeadersíí í
.ííí ì%
ToImmutableDictionaryííì ®
(íí® ©
)íí© ™
)íí™ ´
;íí´ ¨
public
îî 
static
îî  
IHeadersCollection
îî (!
ToHeadersCollection
îî) <
(
îî< =
this
îî= A 
HttpContentHeaders
îîB T
?
îîT U 
httpContentHeaders
îîV h
)
îîh i
=>
ïï 
new
ïï 
HeadersCollection
ïï $
(
ïï$ % 
httpContentHeaders
ïï% 7
==
ïï8 :
null
ïï; ?
?
ïï@ A!
ImmutableDictionary
ïïB U
.
ïïU V
Create
ïïV \
<
ïï\ ]
string
ïï] c
,
ïïc d
IEnumerable
ïïe p
<
ïïp q
string
ïïq w
>
ïïw x
>
ïïx y
(
ïïy z
)
ïïz {
:
ïï| }!
httpContentHeadersïï~ ê
.ïïê ë%
ToImmutableDictionaryïïë ¶
(ïï¶ ß
)ïïß ®
)ïï® ©
;ïï© ™
public
ôô 
static
ôô  
IHeadersCollection
ôô (%
WithBasicAuthentication
ôô) @
(
ôô@ A
this
ôôA E 
IHeadersCollection
ôôF X
?
ôôX Y
requestHeaders
ôôZ h
,
ôôh i
string
ôôj p
userName
ôôq y
,
ôôy z
stringôô{ Å
passwordôôÇ ä
)ôôä ã
=>
öö 
WithHeaderValue
öö 
(
öö 
requestHeaders
öö -
,
öö- .
Authorization
öö/ <
,
öö< =
$str
öö> F
+
ööG H
Convert
ööI P
.
ööP Q
ToBase64String
ööQ _
(
öö_ `
Encoding
öö` h
.
ööh i
UTF8
ööi m
.
ööm n
GetBytes
öön v
(
ööv w
userName
ööw 
+ööÄ Å
$strööÇ Ö
+ööÜ á
passwordööà ê
)ööê ë
)ööë í
)ööí ì
;ööì î
public
úú 
static
úú  
IHeadersCollection
úú (+
WithBearerTokenAuthentication
úú) F
(
úúF G
this
úúG K 
IHeadersCollection
úúL ^
?
úú^ _
requestHeaders
úú` n
,
úún o
string
úúp v
bearerTokenúúw Ç
)úúÇ É
=>
ùù 
WithHeaderValue
ùù 
(
ùù 
requestHeaders
ùù -
,
ùù- .
Authorization
ùù/ <
,
ùù< =
$str
ùù> G
+
ùùH I
bearerToken
ùùJ U
)
ùùU V
;
ùùV W
public
üü 
static
üü  
IHeadersCollection
üü ('
WithJsonContentTypeHeader
üü) B
(
üüB C
this
üüC G 
IHeadersCollection
üüH Z
?
üüZ [
requestHeaders
üü\ j
)
üüj k
=>
†† 

WithHeaderValue
†† 
(
†† 
requestHeaders
†† )
,
††) *#
ContentTypeHeaderName
††+ @
,
††@ A
JsonMediaType
††B O
)
††O P
;
††P Q
public
¢¢ 
static
¢¢  
IHeadersCollection
¢¢ ($
JsonContentTypeHeaders
¢¢) ?
{
¢¢@ A
get
¢¢B E
;
¢¢E F
}
¢¢G H
=
¢¢I J
new
¢¢K N
HeadersCollection
¢¢O `
(
¢¢` a#
ContentTypeHeaderName
¢¢a v
,
¢¢v w
JsonMediaType¢¢x Ö
)¢¢Ö Ü
;¢¢Ü á
}
•• 
}¶¶ Ò
aC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\GlobalSuppressions.cs
[ 
assembly 	
:	 

SuppressMessage 
( 
$str "
," #
$str$ W
,W X
JustificationY f
=g h
$stri t
,t u
Scopev {
=| }
$str	~ â
,
â ä
Target
ã ë
=
í ì
$str
î ß
)
ß ®
]
® © 	
gC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\DeserializationException.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class $
DeserializationException )
:* +
	Exception, 5
{ 
private 
readonly 
byte 
[ 
] 
responseData  ,
;, -
public		 $
DeserializationException		 '
(		' (
string

 
message

 
,

 
byte 
[ 
] 
responseData 
,  
	Exception 
? 
innerException %
)% &
:' (
base) -
(- .
message. 5
,5 6
innerException7 E
)E F
=>G I
thisJ N
.N O
responseDataO [
=\ ]
responseData^ j
;j k
public 
byte 
[ 
] 
GetResponseData %
(% &
)& '
=>( *
responseData+ 7
;7 8
} 
} ˚
_C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\CreateHttpClient.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

delegate 

HttpClient 
CreateHttpClient /
(/ 0
string0 6
name7 ;
); <
;< =
}		 ⁄
bC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\CreateClientOptions.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class 
CreateClientOptions $
{ 
public 
CreateClientOptions "
(" #
CreateHttpClient# 3
createHttpClient4 D
)D E
=>F H
CreateHttpClientI Y
=Z [
createHttpClient\ l
;l m
public

 
AbsoluteUrl

 
BaseUrl

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
=

1 2
AbsoluteUrl

3 >
.

> ?
Empty

? D
;

D E
public !
ISerializationAdapter $ 
SerializationAdapter% 9
{: ;
get< ?
;? @
setA D
;D E
}F G
public 
CreateHttpClient 
CreateHttpClient  0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public #
ISendHttpRequestMessage &"
SendHttpRequestMessage' =
{> ?
get@ C
;C D
setE H
;H I
}J K
public "
IGetHttpRequestMessage %!
GetHttpRequestMessage& ;
{< =
get> A
;A B
setC F
;F G
}H I
public 
IHeadersCollection !!
DefaultRequestHeaders" 7
{8 9
get: =
;= >
set? B
;B C
}D E
public 
bool #
ThrowExceptionOnFailure +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
} 
} ü
[C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\CreateClient.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

delegate 
IClient 
CreateClient (
(( )
string) /
name0 4
,4 5
Action6 <
<< =
CreateClientOptions= P
>P Q
?Q R
configureClientS b
=c d
nulle i
)i j
;j k
} Ÿ’
]C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.Abstractions\CallExtensions.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

static 
class 
CallExtensions &
{		 
public 
static 
Task 
< 
Response #
># $
DeleteAsync% 0
(0 1
this1 5
IClient6 =
client> D
,D E
stringF L
pathM Q
)Q R
=> 
DeleteAsync 
( 
client !
,! "
new# &
RelativeUrl' 2
(2 3
path3 7
)7 8
)8 9
;9 :
public 
static 
async 
Task  
<  !
Response! )
>) *
DeleteAsync+ 6
(6 7
this 
IClient 
client 
,  
RelativeUrl 
? 
resource !
=" #
null$ (
,( )
IHeadersCollection 
? 
requestHeaders  .
=/ 0
null1 5
,5 6
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
if 
( 
client 
== 
null 
) 
throw  %
new& )!
ArgumentNullException* ?
(? @
nameof@ F
(F G
clientG M
)M N
)N O
;O P
var 
response 
= 
( 
Response $
)$ %
await% *
client+ 1
.1 2
	SendAsync2 ;
<; <
object< B
,B C
objectD J
>J K
(K L
new 
Request 
< 
object 
> 
(  
( 
resource 
!= 
null !
?" #
client$ *
.* +
BaseUrl+ 2
?2 3
.3 4
WithRelativeUrl4 C
(C D
resourceD L
)L M
:N O
client 
. 
BaseUrl 
) 
??  "
throw# (
new) ,!
ArgumentNullException- B
(B C
nameofC I
(I J
resourceJ R
)R S
)S T
,T U
null 
, 
client 
. '
AppendDefaultRequestHeaders 2
(2 3
requestHeaders3 A
??B D!
NullHeadersCollectionE Z
.Z [
Instance[ c
)c d
,d e
HttpRequestMethod   !
.  ! "
Delete  " (
,  ( )
cancellationToken!! !
:!!! "
cancellationToken!!# 4
)!!4 5
)!!5 6
."" 
ConfigureAwait"" 
(""  
false""  %
)""% &
;""& '
return$$ 
response$$ 
;$$ 
}%% 	
public++ 
static++ 
Task++ 
<++ 
Response++ #
<++# $
TResponseBody++$ 1
>++1 2
>++2 3
GetAsync++4 <
<++< =
TResponseBody++= J
>++J K
(++K L
this++L P
IClient++Q X
client++Y _
)++_ `
=>,,
 
client,, 
==,, 
null,, 
?,, 
throw,, #
new,,$ '!
ArgumentNullException,,( =
(,,= >
nameof,,> D
(,,D E
client,,E K
),,K L
),,L M
:,,N O
GetAsync-- 
<-- 
TResponseBody-- #
>--# $
(--$ %
client--% +
,--+ ,
client--- 3
.--3 4
BaseUrl--4 ;
.--; <
RelativeUrl--< G
)--G H
;--H I
public// 
static// 
Task// 
<// 
Response// #
<//# $
TResponseBody//$ 1
>//1 2
>//2 3
GetAsync//4 <
<//< =
TResponseBody//= J
>//J K
(//K L
this//L P
IClient//Q X
client//Y _
,//_ `
string//a g
path//h l
)//l m
=>00 
GetAsync00 
<00 
TResponseBody00 %
>00% &
(00& '
client00' -
,00- .
new00/ 2
RelativeUrl003 >
(00> ?
path00? C
)00C D
)00D E
;00E F
public22 
static22 
Task22 
<22 
Response22 #
<22# $
TResponseBody22$ 1
>221 2
>222 3
GetAsync224 <
<22< =
TResponseBody22= J
>22J K
(22K L
this33 
IClient33 
client33 
,33  
RelativeUrl44 
?44 
resource44 !
=44" #
null44$ (
,44( )
IHeadersCollection55 
?55 
requestHeaders55  .
=55/ 0
null551 5
,555 6
CancellationToken66 
cancellationToken66 /
=660 1
default662 9
)669 :
=>66; =
client66> D
==66E G
null66H L
?77 
throw77 
new77 !
ArgumentNullException77 1
(771 2
nameof772 8
(778 9
client779 ?
)77? @
)77@ A
:88 
client88 
.88 
	SendAsync88 "
<88" #
TResponseBody88# 0
,880 1
object882 8
>888 9
(889 :
new99 
Request99 
<99 
object99 "
>99" #
(99# $
(:: 
resource:: 
!=::  
null::! %
?::& '
client::( .
.::. /
BaseUrl::/ 6
.::6 7
WithRelativeUrl::7 F
(::F G
resource::G O
)::O P
:::Q R
client;; 
.;; 
BaseUrl;; "
);;" #
??;;$ &
throw;;' ,
new;;- 0!
ArgumentNullException;;1 F
(;;F G
nameof;;G M
(;;M N
resource;;N V
);;V W
);;W X
,;;X Y
null<< 
,<< 
client== 
.== '
AppendDefaultRequestHeaders== 6
(==6 7
requestHeaders==7 E
??==F H!
NullHeadersCollection==I ^
.==^ _
Instance==_ g
)==g h
,==h i
HttpRequestMethod>> %
.>>% &
Get>>& )
,>>) *
cancellationToken?? %
:??% &
cancellationToken??' 8
)??8 9
)??9 :
;??: ;
publicDD 
staticDD 
TaskDD 
<DD 
ResponseDD #
<DD# $
TResponseBodyDD$ 1
>DD1 2
>DD2 3
GetAsyncDD4 <
<DD< =
TResponseBodyDD= J
>DDJ K
(DDK L
thisEE 
IClientEE 
clientEE 
,EE  
AbsoluteUrlFF 
resourceFF  
,FF  !
IHeadersCollectionGG 
?GG 
requestHeadersGG  .
=GG/ 0
nullGG1 5
,GG5 6
CancellationTokenHH 
cancellationTokenHH /
=HH0 1
defaultHH2 9
)HH9 :
=>HH; =
clientHH> D
==HHE G
nullHHH L
?II 
throwII 
newII !
ArgumentNullExceptionII 1
(II1 2
nameofII2 8
(II8 9
clientII9 ?
)II? @
)II@ A
:JJ 
clientJJ 
.JJ 
	SendAsyncJJ "
<JJ" #
TResponseBodyJJ# 0
,JJ0 1
objectJJ2 8
>JJ8 9
(JJ9 :
newKK 
RequestKK 
<KK 
objectKK "
>KK" #
(KK# $
resourceLL 
,LL 
nullMM 
,MM 
clientNN 
.NN '
AppendDefaultRequestHeadersNN 6
(NN6 7
requestHeadersNN7 E
??NNF H!
NullHeadersCollectionNNI ^
.NN^ _
InstanceNN_ g
)NNg h
,NNh i
HttpRequestMethodOO %
.OO% &
GetOO& )
,OO) *
cancellationTokenPP %
:PP% &
cancellationTokenPP' 8
)PP8 9
)PP9 :
;PP: ;
publicVV 
staticVV 
TaskVV 
<VV 
ResponseVV #
<VV# $
TResponseBodyVV$ 1
>VV1 2
>VV2 3

PatchAsyncVV4 >
<VV> ?
TResponseBodyVV? L
,VVL M
TRequestBodyVVN Z
>VVZ [
(VV[ \
thisWW 
IClientWW 
clientWW 
,WW  
TRequestBodyXX 
requestBodyXX $
,XX$ %
stringYY 
pathYY 
)YY 
=>YY 

PatchAsyncYY &
<YY& '
TResponseBodyYY' 4
,YY4 5
TRequestBodyYY6 B
>YYB C
(YYC D
clientZZ 
,ZZ 
requestBody[[ 
,[[ 
new\\ 
RelativeUrl\\ 
(\\  
path\\  $
)\\$ %
)\\% &
;\\& '
public^^ 
static^^ 
Task^^ 
<^^ 
Response^^ #
<^^# $
TResponseBody^^$ 1
>^^1 2
>^^2 3

PatchAsync^^4 >
<^^> ?
TResponseBody^^? L
>^^L M
(^^M N
this__ 
IClient__ 
client__ 
,__ 
RelativeUrl`` 
?`` 
resource`` 
=`` 
null``  $
,``$ %
IHeadersCollectionaa 
?aa 
requestHeadersaa *
=aa+ ,
nullaa- 1
,aa1 2
CancellationTokenbb 
cancellationTokenbb +
=bb, -
defaultbb. 5
)bb5 6
=>cc 
	SendAsynccc 
<cc 
TResponseBodycc &
,cc& '
objectcc( .
>cc. /
(cc/ 0
clientdd 
,dd 
HttpRequestMethodee !
.ee! "
Patchee" '
,ee' (
defaultff 
,ff 
resourcegg 
,gg 
requestHeadershh 
,hh 
cancellationTokenii !
)ii! "
;ii" #
publicll 
staticll 
asyncll 
Taskll  
<ll  !
Responsell! )
<ll) *
TResponseBodyll* 7
>ll7 8
>ll8 9

PatchAsyncll: D
<llD E
TResponseBodyllE R
,llR S
TRequestBodyllT `
>ll` a
(lla b
thismm 
IClientmm 
clientmm 
,mm 
TRequestBodynn 
requestnn 
,nn 
TimeSpanoo 
timeoutoo 
,oo 
RelativeUrlpp 
?pp 
resourcepp 
=pp 
nullpp  $
,pp$ %
IHeadersCollectionqq 
?qq 
requestHeadersqq *
=qq+ ,
nullqq- 1
)qq1 2
{rr 	
usingss 
varss #
cancellationTokenSourcess -
=ss. /
newss0 3#
CancellationTokenSourcess4 K
(ssK L
timeoutssL S
)ssS T
;ssT U
returnuu 
awaituu 
	SendAsyncuu "
<uu" #
TResponseBodyuu# 0
,uu0 1
objectuu2 8
>uu8 9
(uu9 :
clientvv 
,vv 
HttpRequestMethodww !
.ww! "
Patchww" '
,ww' (
requestxx 
,xx 
resourceyy 
,yy 
requestHeaderszz 
,zz #
cancellationTokenSource{{ '
.{{' (
Token{{( -
){{- .
.{{. /
ConfigureAwait{{/ =
({{= >
false{{> C
){{C D
;{{D E
}|| 	
public~~ 
static~~ 
Task~~ 
<~~ 
Response~~ #
<~~# $
TResponseBody~~$ 1
>~~1 2
>~~2 3

PatchAsync~~4 >
<~~> ?
TResponseBody~~? L
,~~L M
TRequestBody~~N Z
>~~Z [
(~~[ \
this 
IClient 
client 
,  
TRequestBody
ÄÄ 
?
ÄÄ 
requestBody
ÄÄ %
=
ÄÄ& '
default
ÄÄ( /
,
ÄÄ/ 0
RelativeUrl
ÅÅ 
?
ÅÅ 
resource
ÅÅ !
=
ÅÅ" #
null
ÅÅ$ (
,
ÅÅ( ) 
IHeadersCollection
ÇÇ 
?
ÇÇ 
requestHeaders
ÇÇ  .
=
ÇÇ/ 0
null
ÇÇ1 5
,
ÇÇ5 6
CancellationToken
ÉÉ 
cancellationToken
ÉÉ /
=
ÉÉ0 1
default
ÉÉ2 9
)
ÉÉ9 :
=>
ÑÑ 
	SendAsync
ÑÑ 
<
ÑÑ 
TResponseBody
ÑÑ &
,
ÑÑ& '
TRequestBody
ÑÑ( 4
>
ÑÑ4 5
(
ÑÑ5 6
client
ÖÖ 
,
ÖÖ 
HttpRequestMethod
ÜÜ !
.
ÜÜ! "
Patch
ÜÜ" '
,
ÜÜ' (
requestBody
áá 
,
áá 
resource
àà 
,
àà 
requestHeaders
ââ 
,
ââ 
cancellationToken
ää !
)
ää! "
;
ää" #
public
êê 
static
êê 
Task
êê 
<
êê 
Response
êê #
<
êê# $
TResponseBody
êê$ 1
>
êê1 2
>
êê2 3
	PostAsync
êê4 =
<
êê= >
TResponseBody
êê> K
,
êêK L
TRequestBody
êêM Y
>
êêY Z
(
êêZ [
this
ëë 
IClient
ëë 
client
ëë 
,
ëë  
TRequestBody
íí 
requestBody
íí $
)
íí$ %
=>
ìì 
	PostAsync
ìì 
<
ìì 
TResponseBody
ìì &
,
ìì& '
TRequestBody
ìì( 4
>
ìì4 5
(
ìì5 6
client
îî 
,
îî 
requestBody
ïï 
,
ïï 
null
ïï !
,
ïï! "
null
ïï# '
,
ïï' (
default
ïï) 0
)
ïï0 1
;
ïï1 2
public
óó 
static
óó 
Task
óó 
<
óó 
Response
óó #
<
óó# $
TResponseBody
óó$ 1
>
óó1 2
>
óó2 3
	PostAsync
óó4 =
<
óó= >
TResponseBody
óó> K
,
óóK L
TRequestBody
óóM Y
>
óóY Z
(
óóZ [
this
òò 
IClient
òò 
client
òò 
,
òò  
TRequestBody
ôô 
requestBody
ôô $
,
ôô$ %
string
öö 
path
öö 
)
öö 
=>
õõ 
	PostAsync
õõ 
<
õõ 
TResponseBody
õõ &
,
õõ& '
TRequestBody
õõ( 4
>
õõ4 5
(
õõ5 6
client
úú 
,
úú 
requestBody
ùù 
,
ùù 
new
ûû 
RelativeUrl
ûû 
(
ûû  
path
ûû  $
)
ûû$ %
)
ûû% &
;
ûû& '
public
†† 
static
†† 
Task
†† 
<
†† 
Response
†† #
<
††# $
TResponseBody
††$ 1
>
††1 2
>
††2 3
	PostAsync
††4 =
<
††= >
TResponseBody
††> K
>
††K L
(
††L M
this
°° 
IClient
°° 
client
°° 
,
°°  
RelativeUrl
¢¢ 
?
¢¢ 
resource
¢¢ !
=
¢¢" #
null
¢¢$ (
,
¢¢( ) 
IHeadersCollection
££ 
?
££ 
requestHeaders
££  .
=
££/ 0
null
££1 5
,
££5 6
CancellationToken
§§ 
cancellationToken
§§ /
=
§§0 1
default
§§2 9
)
§§9 :
=>
•• 
	SendAsync
•• 
<
•• 
TResponseBody
•• &
,
••& '
object
••( .
>
••. /
(
••/ 0
client
¶¶ 
,
¶¶ 
HttpRequestMethod
ßß !
.
ßß! "
Post
ßß" &
,
ßß& '
default
®® 
,
®® 
resource
©© 
,
©© 
requestHeaders
™™ 
,
™™ 
cancellationToken
´´ !
)
´´! "
;
´´" #
public
≠≠ 
static
≠≠ 
Task
≠≠ 
<
≠≠ 
Response
≠≠ #
<
≠≠# $
TResponseBody
≠≠$ 1
>
≠≠1 2
>
≠≠2 3
	PostAsync
≠≠4 =
<
≠≠= >
TResponseBody
≠≠> K
,
≠≠K L
TRequestBody
≠≠M Y
>
≠≠Y Z
(
≠≠Z [
this
ÆÆ 
IClient
ÆÆ 
client
ÆÆ 
,
ÆÆ  
TRequestBody
ØØ 
?
ØØ 
requestBody
ØØ %
=
ØØ& '
default
ØØ( /
,
ØØ/ 0
RelativeUrl
∞∞ 
?
∞∞ 
resource
∞∞ !
=
∞∞" #
null
∞∞$ (
,
∞∞( ) 
IHeadersCollection
±± 
?
±± 
requestHeaders
±±  .
=
±±/ 0
null
±±1 5
,
±±5 6
CancellationToken
≤≤ 
cancellationToken
≤≤ /
=
≤≤0 1
default
≤≤2 9
)
≤≤9 :
=>
≥≥ 
	SendAsync
≥≥ 
<
≥≥ 
TResponseBody
≥≥ &
,
≥≥& '
TRequestBody
≥≥( 4
>
≥≥4 5
(
≥≥5 6
client
¥¥ 
,
¥¥ 
HttpRequestMethod
µµ !
.
µµ! "
Post
µµ" &
,
µµ& '
requestBody
∂∂ 
,
∂∂ 
resource
∑∑ 
,
∑∑ 
requestHeaders
∏∏ 
,
∏∏ 
cancellationToken
ππ !
)
ππ! "
;
ππ" #
public
øø 
static
øø 
Task
øø 
<
øø 
Response
øø #
<
øø# $
TResponseBody
øø$ 1
>
øø1 2
>
øø2 3
PutAsync
øø4 <
<
øø< =
TResponseBody
øø= J
,
øøJ K
TRequestBody
øøL X
>
øøX Y
(
øøY Z
this
¿¿ 
IClient
¿¿ 
client
¿¿ 
,
¿¿  
TRequestBody
¡¡ 
requestBody
¡¡ $
,
¡¡$ %
string
¬¬ 
path
¬¬ 
)
¬¬ 
=>
¬¬ 
PutAsync
¬¬ $
<
¬¬$ %
TResponseBody
¬¬% 2
,
¬¬2 3
TRequestBody
¬¬4 @
>
¬¬@ A
(
¬¬A B
client
√√ 
,
√√ 
requestBody
ƒƒ 
,
ƒƒ 
new
≈≈ 
RelativeUrl
≈≈ 
(
≈≈  
path
≈≈  $
)
≈≈$ %
)
≈≈% &
;
≈≈& '
public
«« 
static
«« 
Task
«« 
<
«« 
Response
«« #
<
««# $
TResponseBody
««$ 1
>
««1 2
>
««2 3
PutAsync
««4 <
<
««< =
TResponseBody
««= J
>
««J K
(
««K L
this
»» 
IClient
»» 
client
»» 
,
»»  
RelativeUrl
…… 
?
…… 
resource
…… !
=
……" #
null
……$ (
,
……( ) 
IHeadersCollection
   
?
   
requestHeaders
    .
=
  / 0
null
  1 5
,
  5 6
CancellationToken
ÀÀ 
cancellationToken
ÀÀ /
=
ÀÀ0 1
default
ÀÀ2 9
)
ÀÀ9 :
=>
ÃÃ 
PutAsync
ÃÃ 
<
ÃÃ 
TResponseBody
ÃÃ %
,
ÃÃ% &
object
ÃÃ' -
>
ÃÃ- .
(
ÃÃ. /
client
ÕÕ 
,
ÕÕ 
HttpRequestMethod
ŒŒ !
.
ŒŒ! "
Put
ŒŒ" %
,
ŒŒ% &
resource
œœ 
,
œœ 
requestHeaders
–– 
,
–– 
cancellationToken
—— !
)
——! "
;
——" #
public
”” 
static
”” 
Task
”” 
<
”” 
Response
”” #
<
””# $
TResponseBody
””$ 1
>
””1 2
>
””2 3
PutAsync
””4 <
<
””< =
TResponseBody
””= J
,
””J K
TRequestBody
””L X
>
””X Y
(
””Y Z
this
‘‘ 
IClient
‘‘ 
client
‘‘ 
,
‘‘  
TRequestBody
’’ 
?
’’ 
requestBody
’’ %
=
’’& '
default
’’( /
,
’’/ 0
RelativeUrl
÷÷ 
?
÷÷ 
resource
÷÷ !
=
÷÷" #
null
÷÷$ (
,
÷÷( ) 
IHeadersCollection
◊◊ 
?
◊◊ 
requestHeaders
◊◊  .
=
◊◊/ 0
null
◊◊1 5
,
◊◊5 6
CancellationToken
ÿÿ 
cancellationToken
ÿÿ /
=
ÿÿ0 1
default
ÿÿ2 9
)
ÿÿ9 :
=>
ŸŸ 
	SendAsync
ŸŸ 
<
ŸŸ 
TResponseBody
ŸŸ &
,
ŸŸ& '
TRequestBody
ŸŸ( 4
>
ŸŸ4 5
(
ŸŸ5 6
client
⁄⁄ 
,
⁄⁄ 
HttpRequestMethod
€€ !
.
€€! "
Put
€€" %
,
€€% &
requestBody
‹‹ 
,
‹‹ 
resource
›› 
,
›› 
requestHeaders
ﬁﬁ 
,
ﬁﬁ 
cancellationToken
ﬂﬂ !
)
ﬂﬂ! "
;
ﬂﬂ" #
public
‰‰ 
static
‰‰ 
Task
‰‰ 
<
‰‰ 
Response
‰‰ #
<
‰‰# $
TResponseBody
‰‰$ 1
>
‰‰1 2
>
‰‰2 3
	SendAsync
‰‰4 =
<
‰‰= >
TResponseBody
‰‰> K
,
‰‰K L
TRequestBody
‰‰M Y
>
‰‰Y Z
(
‰‰Z [
IClient
ÂÂ 
client
ÂÂ 
,
ÂÂ 
HttpRequestMethod
ÊÊ 
httpRequestMethod
ÊÊ /
,
ÊÊ/ 0
TRequestBody
ÁÁ 
?
ÁÁ 
requestBody
ÁÁ %
,
ÁÁ% &
RelativeUrl
ËË 
?
ËË 
resource
ËË !
,
ËË! " 
IHeadersCollection
ÈÈ 
?
ÈÈ 
requestHeaders
ÈÈ  .
=
ÈÈ/ 0
null
ÈÈ1 5
,
ÈÈ5 6
CancellationToken
ÍÍ 
cancellationToken
ÍÍ /
=
ÍÍ0 1
default
ÍÍ2 9
)
ÍÍ9 :
{
ÎÎ 	
if
ÏÏ 
(
ÏÏ 
client
ÏÏ 
==
ÏÏ 
null
ÏÏ 
)
ÏÏ 
throw
ÏÏ  %
new
ÏÏ& )#
ArgumentNullException
ÏÏ* ?
(
ÏÏ? @
nameof
ÏÏ@ F
(
ÏÏF G
client
ÏÏG M
)
ÏÏM N
)
ÏÏN O
;
ÏÏO P
requestHeaders
ÓÓ 
=
ÓÓ 
client
ÓÓ #
.
ÓÓ# $)
AppendDefaultRequestHeaders
ÓÓ$ ?
(
ÓÓ? @
requestHeaders
ÓÓ@ N
??
ÓÓO Q#
NullHeadersCollection
ÓÓR g
.
ÓÓg h
Instance
ÓÓh p
)
ÓÓp q
;
ÓÓq r
return
 
	SendAsync
 
<
 
TResponseBody
 *
,
* +
TRequestBody
, 8
>
8 9
(
9 :
client
ÒÒ 
,
ÒÒ 
resource
ÚÚ 
,
ÚÚ 
requestBody
ÛÛ 
,
ÛÛ 
requestHeaders
ÙÙ 
,
ÙÙ 
httpRequestMethod
ıı !
,
ıı! "
cancellationToken
ˆˆ !
)
ˆˆ! "
;
ˆˆ" #
}
˜˜ 	
public
˘˘ 
static
˘˘ 
Task
˘˘ 
<
˘˘ 
Response
˘˘ #
<
˘˘# $
TResponseBody
˘˘$ 1
>
˘˘1 2
>
˘˘2 3
	SendAsync
˘˘4 =
<
˘˘= >
TResponseBody
˘˘> K
,
˘˘K L
TRequestBody
˘˘M Y
>
˘˘Y Z
(
˘˘Z [
IClient
˙˙ 
client
˙˙ 
,
˙˙ 
RelativeUrl
˚˚ 
?
˚˚ 
resource
˚˚ !
,
˚˚! "
TRequestBody
¸¸ 
?
¸¸ 
requestBodyData
¸¸ )
,
¸¸) * 
IHeadersCollection
˝˝ 
requestHeaders
˝˝ -
,
˝˝- .
HttpRequestMethod
˛˛ 
httpRequestMethod
˛˛ /
,
˛˛/ 0
CancellationToken
ˇˇ 
cancellationToken
ˇˇ /
)
ˇˇ/ 0
=>
ÄÄ 
client
ÅÅ 
!=
ÅÅ 
null
ÅÅ 
?
ÅÅ 
	SendAsync
ÅÅ '
<
ÅÅ' (
TResponseBody
ÅÅ( 5
,
ÅÅ5 6
TRequestBody
ÅÅ7 C
>
ÅÅC D
(
ÅÅD E
client
ÅÅE K
,
ÅÅK L
new
ÇÇ 
Request
ÇÇ  '
<
ÇÇ' (
TRequestBody
ÇÇ( 4
>
ÇÇ4 5
(
ÇÇ5 6
(
ÉÉ  !
resource
ÉÉ! )
!=
ÉÉ* ,
null
ÉÉ- 1
?
ÉÉ2 3
client
ÉÉ4 :
.
ÉÉ: ;
BaseUrl
ÉÉ; B
?
ÉÉB C
.
ÉÉC D
WithRelativeUrl
ÉÉD S
(
ÉÉS T
resource
ÉÉT \
)
ÉÉ\ ]
:
ÉÉ^ _
client
ÑÑ  &
.
ÑÑ& '
BaseUrl
ÑÑ' .
)
ÑÑ. /
??
ÑÑ0 2
throw
ÑÑ3 8
new
ÑÑ9 <#
ArgumentNullException
ÑÑ= R
(
ÑÑR S
nameof
ÑÑS Y
(
ÑÑY Z
resource
ÑÑZ b
)
ÑÑb c
)
ÑÑc d
,
ÑÑd e
requestBodyData
ÖÖ  /
,
ÖÖ/ 0
requestHeaders
ÜÜ  .
,
ÜÜ. /
httpRequestMethod
áá  1
,
áá1 2
cancellationToken
àà  1
:
àà1 2
cancellationToken
àà3 D
)
ààD E
)
ààE F
:
ààG H
throw
ààI N
new
ààO R#
ArgumentNullException
ààS h
(
ààh i
nameof
àài o
(
àào p
client
ààp v
)
ààv w
)
ààw x
;
ààx y
public
ãã 
static
ãã 
Task
ãã 
<
ãã 
Response
ãã #
<
ãã# $
TResponseBody
ãã$ 1
>
ãã1 2
>
ãã2 3
	SendAsync
ãã4 =
<
ãã= >
TResponseBody
ãã> K
,
ããK L
TRequestBody
ããM Y
>
ããY Z
(
ããZ [
this
åå 
IClient
åå 
client
åå 
,
åå  
IRequest
çç 
<
çç 
TRequestBody
çç !
>
çç! "
request
çç# *
)
çç* +
=>
éé 
client
èè 
==
èè 
null
èè 
?
èè 
throw
èè "
new
èè# &#
ArgumentNullException
èè' <
(
èè< =
nameof
èè= C
(
èèC D
client
èèD J
)
èèJ K
)
èèK L
:
èèM N
request
èèO V
!=
èèW Y
null
èèZ ^
?
èè_ `
client
èèa g
.
èèg h
	SendAsync
èèh q
<
èèq r
TResponseBody
èèr 
,èè Ä
TRequestBodyèèÅ ç
>èèç é
(èèé è
requestèèè ñ
)èèñ ó
:èèò ô
throwèèö ü
newèè† £%
ArgumentNullExceptionèè§ π
(èèπ ∫
nameofèè∫ ¿
(èè¿ ¡
requestèè¡ »
)èè» …
)èè…  
;èè  À
}
îî 
}ïï 