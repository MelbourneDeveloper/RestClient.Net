Ô
GC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\Stuff.cs
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
]_ `Ú

\C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\SingletonHttpClientFactory.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

sealed		 
class		 &
SingletonHttpClientFactory		 2
:		3 4
IDisposable		5 @
{

 
private 
bool 
disposed 
; 
public 

HttpClient 

HttpClient $
{% &
get' *
;* +
}, -
public &
SingletonHttpClientFactory )
() *

HttpClient* 4

httpClient5 ?
)? @
=>A C

HttpClientD N
=O P

httpClientQ [
;[ \
public 

HttpClient 
CreateClient &
(& '
string' -
name. 2
)2 3
=>4 6

HttpClient7 A
;A B
public 
void 
Dispose 
( 
) 
{ 	
if   
(   
disposed   
)   
return    
;    !
disposed!! 
=!! 
true!! 
;!! 

HttpClient"" 
."" 
Dispose"" 
("" 
)""  
;""  !
}## 	
}%% 
}'' √
OC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\SendException.cs
	namespace 	

RestClient
 
. 
Net 
{ 
[ 
Serializable 
] 
public 

class 
SendException 
:  
	Exception! *
{ 
public 
IRequest 
Request 
{  !
get" %
;% &
}' (
public

 
SendException

 
(

 
string

 #
message

$ +
,

+ ,
IRequest

- 5
request

6 =
,

= >
	Exception

? H
innerException

I W
)

W X
:

Y Z
base

[ _
(

_ `
message

` g
,

g h
innerException

i w
)

w x
=>

y {
Request	

| É
=


Ñ Ö
request


Ü ç
;


ç é
} 
} ä
TC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\GlobalSuppressions.cs
[ 
assembly 	
:	 

System 
. 
Diagnostics 
. 
CodeAnalysis *
.* +
SuppressMessage+ :
(: ;
$str; C
,C D
$strE m
,m n
Justificationo |
=} ~
$str	 ä
,
ä ã
Scope
å ë
=
í ì
$str
î ú
,
ú ù
Target
û §
=
• ¶
$str
ß ›
)
› ﬁ
]
ﬁ ﬂ
[ 
assembly 	
:	 

System 
. 
Diagnostics 
. 
CodeAnalysis *
.* +
SuppressMessage+ :
(: ;
$str; B
,B C
$strD y
,y z
Justification	{ à
=
â ä
$str
ã ñ
,
ñ ó
Scope
ò ù
=
û ü
$str
† ®
,
® ©
Target
™ ∞
=
± ≤
$str
≥ È
)
È Í
]
Í Î
[		 
assembly		 	
:			 

System		 
.		 
Diagnostics		 
.		 
CodeAnalysis		 *
.		* +
SuppressMessage		+ :
(		: ;
$str		; C
,		C D
$str		E w
,		w x
Justification			y Ü
=
		á à
$str
		â î
,
		î ï
Scope
		ñ õ
=
		ú ù
$str
		û §
,
		§ •
Target
		¶ ¨
=
		≠ Æ
$str
		Ø –
)
		– —
]
		— “
[

 
assembly

 	
:

	 

System

 
.

 
Diagnostics

 
.

 
CodeAnalysis

 *
.

* +
SuppressMessage

+ :
(

: ;
$str

; B
,

B C
$str

D q
,

q r
Justification	

s Ä
=


Å Ç
$str


É é
,


é è
Scope


ê ï
=


ñ ó
$str


ò û
,


û ü
Target


† ¶
=


ß ®
$str


©  
)


  À
]


À Ãó-
_C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\DefaultSendHttpRequestMessage.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

class		 )
DefaultSendHttpRequestMessage		 .
:		/ 0#
ISendHttpRequestMessage		1 H
{

 
public 
static )
DefaultSendHttpRequestMessage 3
Instance4 <
{= >
get? B
;B C
}D E
=F G
newH K)
DefaultSendHttpRequestMessageL i
(i j
)j k
;k l
public 
async 
Task 
< 
HttpResponseMessage -
>- ."
SendHttpRequestMessage/ E
<E F
TRequestBodyF R
>R S
(S T

HttpClient 

httpClient !
,! ""
IGetHttpRequestMessage ""
httpRequestMessageFunc# 9
,9 :
IRequest 
< 
TRequestBody !
>! "
request# *
,* +
ILogger 
logger 
, !
ISerializationAdapter ! 
serializationAdapter" 6
)6 7
{ 	
if 
( 

httpClient 
== 
null "
)" #
throw$ )
new* -!
ArgumentNullException. C
(C D
nameofD J
(J K

httpClientK U
)U V
)V W
;W X
if 
( "
httpRequestMessageFunc &
==' )
null* .
). /
throw0 5
new6 9!
ArgumentNullException: O
(O P
nameofP V
(V W"
httpRequestMessageFuncW m
)m n
)n o
;o p
if 
( 
request 
== 
default "
)" #
throw$ )
new* -!
ArgumentNullException. C
(C D
nameofD J
(J K
requestK R
)R S
)S T
;T U
logger 
??= 

NullLogger !
.! "
Instance" *
;* +
try 
{ 
var 
httpRequestMessage &
=' ("
httpRequestMessageFunc) ?
.? @!
GetHttpRequestMessage@ U
(U V
requestV ]
,] ^
logger_ e
,e f 
serializationAdapterg {
){ |
;| }
logger 
. 
LogTrace 
(  
Messages  (
.( ) 
InfoAttemptingToSend) =
,= >
request? F
)F G
;G H
try!! 
{"" 
var## 
httpResponseMessage## +
=##, -
await##. 3

httpClient##4 >
.##> ?
	SendAsync##? H
(##H I
httpRequestMessage##I [
,##[ \
request##] d
.##d e
CancellationToken##e v
)##v w
.##w x
ConfigureAwait	##x Ü
(
##Ü á
false
##á å
)
##å ç
;
##ç é
logger%% 
.%% 
LogInformation%% )
(%%) *
Messages%%* 2
.%%2 3'
InfoSendReturnedNoException%%3 N
)%%N O
;%%O P
return'' 
httpResponseMessage'' .
;''. /
})) 
catch** 
(** 
ArgumentException** (
aex**) ,
)**, -
{++ 
if,, 
(,, 
aex,, 
.,, 
Message,, #
==,,$ &
$str,,' c
),,c d
{-- 
var.. 
	isRequest.. %
=..& '
httpRequestMessage..( :
...: ;
Content..; B
?..B C
...C D
Headers..D K
...K L
ContentType..L W
==..X Z
null..[ _
;.._ `
var// 
errorTypePart// )
=//* +
	isRequest//, 5
?//6 7
$"//8 :
$str//: U
{//U V
HeadersExtensions//V g
.//g h!
ContentTypeHeaderName//h }
}//} ~
$str	//~ Ö
"
//Ö Ü
:
//á à
$"00 
$str00 2
{002 3
HeadersExtensions003 D
.00D E!
ContentTypeHeaderName00E Z
}00Z [
$str	00[ ç
"
00ç é
;
00é è
throw11 
new22 "
MissingHeaderException22  6
(226 7
$"33  "
$str33" D
{33D E
errorTypePart33E R
}33R S
"33S T
,33T U
	isRequest44  )
,44) *
aex55  #
)55# $
;55$ %
;55& '
}66 
throw77 
;77 
}88 
}99 
catch:: 
(:: &
OperationCanceledException:: -
oce::. 1
)::1 2
{;; 
logger<< 
.<< 
LogError<< 
(<<  
oce<<  #
,<<# $
Messages<<% -
.<<- .*
ErrorMessageOperationCancelled<<. L
,<<L M
request<<N U
)<<U V
;<<V W
throw== 
;== 
}>> 
catch?? 
(?? 
	Exception?? 
ex?? 
)??  
{@@ 
loggerAA 
.AA 
LogErrorAA 
(AA  
exAA  "
,AA" #
MessagesAA$ ,
.AA, -
ErrorOnSendAA- 8
,AA8 9
requestAA: A
)AAA B
;AAB C
throwCC 
;CC 
}DD 
}EE 	
}FF 
}GG é!
ZC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\DefaultHttpClientFactory.cs
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
sealed

 
class

 $
DefaultHttpClientFactory

 0
:

1 2
IDisposable

3 >
{ 
private 
bool 
disposed 
; 
private 
readonly  
ConcurrentDictionary -
<- .
string. 4
,4 5
Lazy6 :
<: ;

HttpClient; E
>E F
>F G
httpClientsH S
;S T
private 
readonly 
Func 
< 
string $
,$ %
Lazy& *
<* +

HttpClient+ 5
>5 6
>6 7
createClientFunc8 H
;H I
private 
readonly 
ILogger  
<  !$
DefaultHttpClientFactory! 9
>9 :
logger; A
;A B
public $
DefaultHttpClientFactory '
(' (
Func( ,
<, -
string- 3
,3 4
Lazy5 9
<9 :

HttpClient: D
>D E
>E F
?F G
createClientFuncH X
=Y Z
null[ _
,_ `
ILoggera h
<h i%
DefaultHttpClientFactory	i Å
>
Å Ç
?
Ç É
logger
Ñ ä
=
ã å
null
ç ë
)
ë í
{ 	
httpClients 
= 
new  
ConcurrentDictionary 2
<2 3
string3 9
,9 :
Lazy; ?
<? @

HttpClient@ J
>J K
>K L
(L M
)M N
;N O
this 
. 
logger 
= 
logger  
??! #

NullLogger$ .
<. /$
DefaultHttpClientFactory/ G
>G H
.H I
InstanceI Q
;Q R
this 
. 
createClientFunc !
=" #
createClientFunc$ 4
??5 7
(8 9
name9 =
=>> @
newA D
LazyE I
<I J

HttpClientJ T
>T U
(U V
(V W
)W X
=>Y [
{ 
this 
. 
logger 
. 
LogInformation *
(* +
$str+ F
,F G
nameH L
)L M
;M N
return 
new 

HttpClient %
(% &
)& '
;' (
} 
,  
LazyThreadSafetyMode #
.# $#
ExecutionAndPublication$ ;
); <
)< =
;= >
} 	
public## 

HttpClient## 
CreateClient## &
(##& '
string##' -
name##. 2
)##2 3
=>$$ 
name$$ 
==$$ 
null$$ 
?$$ 
throw$$ #
new$$$ '!
ArgumentNullException$$( =
($$= >
nameof$$> D
($$D E
name$$E I
)$$I J
)$$J K
:$$L M
httpClients$$N Y
.$$Y Z
GetOrAdd$$Z b
($$b c
name$$c g
,$$g h
createClientFunc$$i y
)$$y z
.$$z {
Value	$${ Ä
;
$$Ä Å
public&& 
void&& 
Dispose&& 
(&& 
)&& 
{'' 	
if(( 
((( 
disposed(( 
)(( 
return((  
;((  !
disposed)) 
=)) 
true)) 
;)) 
foreach++ 
(++ 
var++ 
name++ 
in++  
httpClients++! ,
.++, -
Keys++- 1
)++1 2
{,, 
httpClients-- 
[-- 
name--  
]--  !
.--! "
Value--" '
.--' (
Dispose--( /
(--/ 0
)--0 1
;--1 2
}.. 
}// 	
}11 
}22 „9
^C:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\DefaultGetHttpRequestMessage.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class (
DefaultGetHttpRequestMessage -
:. /"
IGetHttpRequestMessage0 F
{ 
public 
static (
DefaultGetHttpRequestMessage 2
Instance3 ;
{< =
get> A
;A B
}C D
=E F
newG J(
DefaultGetHttpRequestMessageK g
(g h
)h i
;i j
public 
HttpRequestMessage !!
GetHttpRequestMessage" 7
<7 8
T8 9
>9 :
(: ;
IRequest; C
<C D
TD E
>E F
requestG N
,N O
ILoggerP W
loggerX ^
,^ _!
ISerializationAdapter` u!
serializationAdapter	v ä
)
ä ã
{ 	
if 
( 
request 
== 
null 
)  
throw! &
new' *!
ArgumentNullException+ @
(@ A
nameofA G
(G H
requestH O
)O P
)P Q
;Q R
if 
(  
serializationAdapter $
==% '
null( ,
), -
throw. 3
new4 7!
ArgumentNullException8 M
(M N
nameofN T
(T U 
serializationAdapterU i
)i j
)j k
;k l
logger 
??= 

NullLogger !
.! "
Instance" *
;* +
try 
{ 
logger 
. 
LogTrace 
(  
$str  n
,n o

TraceEventp z
.z {
Information	{ Ü
,
Ü á
request
à è
)
è ê
;
ê ë
var 

httpMethod 
=  
string! '
.' (
IsNullOrEmpty( 5
(5 6
request6 =
.= >#
CustomHttpRequestMethod> U
)U V
? 
request 
. 
HttpRequestMethod /
switch0 6
{ 
HttpRequestMethod   )
.  ) *
Get  * -
=>  . 0

HttpMethod  1 ;
.  ; <
Get  < ?
,  ? @
HttpRequestMethod!! )
.!!) *
Post!!* .
=>!!/ 1

HttpMethod!!2 <
.!!< =
Post!!= A
,!!A B
HttpRequestMethod"" )
."") *
Put""* -
=>"". 0

HttpMethod""1 ;
.""; <
Put""< ?
,""? @
HttpRequestMethod## )
.##) *
Delete##* 0
=>##1 3

HttpMethod##4 >
.##> ?
Delete##? E
,##E F
HttpRequestMethod$$ )
.$$) *
Patch$$* /
=>$$0 2
new$$3 6

HttpMethod$$7 A
($$A B
$str$$B I
)$$I J
,$$J K
HttpRequestMethod%% )
.%%) *
Custom%%* 0
=>%%1 3
throw%%4 9
new%%: =%
InvalidOperationException%%> W
(%%W X
$str	%%X ú
)
%%ú ù
,
%%ù û
_&& 
=>&& 
throw&& "
new&&# &%
InvalidOperationException&&' @
(&&@ A
$str&&A S
)&&S T
}'' 
:(( 
new(( 

HttpMethod(( $
((($ %
request((% ,
.((, -#
CustomHttpRequestMethod((- D
)((D E
;((E F
var** 
httpRequestMessage** &
=**' (
new**) ,
HttpRequestMessage**- ?
{++ 
Method,, 
=,, 

httpMethod,, '
,,,' (

RequestUri-- 
=--  
request--! (
.--( )
Uri--) ,
}.. 
;.. 
ByteArrayContent00  
?00  !
httpContent00" -
=00. /
null000 4
;004 5
if11 
(11 
request11 
.11 
BodyData11 $
!=11% '
null11( ,
)11, -
{22 
var33 
bodyDataData33 $
=33% & 
serializationAdapter33' ;
.33; <
	Serialize33< E
(33E F
request33F M
.33M N
BodyData33N V
,33V W
request33X _
.33_ `
Headers33` g
)33g h
;33h i
httpContent44 
=44  !
new44" %
ByteArrayContent44& 6
(446 7
bodyDataData447 C
)44C D
;44D E
httpRequestMessage55 &
.55& '
Content55' .
=55/ 0
httpContent551 <
;55< =
logger66 
.66 
LogTrace66 #
(66# $
$str66$ Z
,66Z [
bodyDataData66\ h
.66h i
Length66i o
)66o p
;66p q
}77 
else88 
{99 
logger:: 
.:: 
LogTrace:: #
(::# $
$str::$ U
)::U V
;::V W
};; 
if== 
(== 
request== 
.== 
Headers== #
!===$ &
null==' +
)==+ ,
{>> 
foreach?? 
(?? 
var??  

headerName??! +
in??, .
request??/ 6
.??6 7
Headers??7 >
.??> ?
Names??? D
)??D E
{@@ 
ifAA 
(AA 
HeadersExtensionsAA -
.AA- .
ContentHeaderNamesAA. @
.AA@ A
ContainsAAA I
(AAI J

headerNameAAJ T
,AAT U
StringComparerAAV d
.AAd e
OrdinalIgnoreCaseAAe v
)AAv w
)AAw x
{CC 
httpContentGG '
?GG' (
.GG( )
HeadersGG) 0
.GG0 1
AddGG1 4
(GG4 5

headerNameGG5 ?
,GG? @
requestGGA H
.GGH I
HeadersGGI P
[GGP Q

headerNameGGQ [
]GG[ \
)GG\ ]
;GG] ^
}HH 
elseII 
{JJ 
httpRequestMessageKK .
.KK. /
HeadersKK/ 6
.KK6 7
AddKK7 :
(KK: ;

headerNameKK; E
,KKE F
requestKKG N
.KKN O
HeadersKKO V
[KKV W

headerNameKKW a
]KKa b
)KKb c
;KKc d
}LL 
}MM 
loggerOO 
.OO 
LogTraceOO #
(OO# $
$strOO$ >
)OO> ?
;OO? @
}PP 
loggerRR 
.RR 
LogTraceRR 
(RR  
$strRR  W
)RRW X
;RRX Y
returnTT 
httpRequestMessageTT )
;TT) *
}UU 
catchVV 
(VV 
	ExceptionVV 
exVV 
)VV  
{WW 
loggerXX 
.XX 
LogErrorXX 
(XX  
exXX  "
,XX" #
$strXX$ ^
,XX^ _

TraceEventXX` j
.XXj k
RequestXXk r
)XXr s
;XXs t
throwZZ 
;ZZ 
}[[ 
}\\ 	
}]] 
}^^ Ö&
OC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\ClientFactory.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

class		 
ClientFactory		 
{

 
private 
readonly 
Func 
< 
string $
,$ %
CreateClientOptions& 9
,9 :
IClient; B
>B C
createClientFuncD T
;T U
private 
readonly  
ConcurrentDictionary -
<- .
string. 4
,4 5
IClient6 =
>= >
clients? F
;F G
private 
readonly 
CreateHttpClient )
createHttpClient* :
;: ;
private 
readonly 
ILoggerFactory '
loggerFactory( 5
;5 6
public 
ClientFactory 
( 
CreateHttpClient 
createHttpClient -
,- .
ILoggerFactory 
? 
loggerFactory )
=* +
null, 0
,0 1
Func 
< 
string 
, 
CreateClientOptions ,
,, -
IClient. 5
>5 6
?6 7
createClientFunc8 H
=I J
nullK O
)O P
{ 	
this 
. 
createHttpClient !
=" #
createHttpClient$ 4
;4 5
this 
. 
loggerFactory 
=  
loggerFactory! .
??/ 1
NullLoggerFactory2 C
.C D
InstanceD L
;L M
clients 
= 
new  
ConcurrentDictionary .
<. /
string/ 5
,5 6
IClient7 >
>> ?
(? @
)@ A
;A B
this 
. 
createClientFunc !
=" #
createClientFunc$ 4
??5 7
new8 ;
Func< @
<@ A
stringA G
,G H
CreateClientOptionsI \
,\ ]
IClient^ e
>e f
(f g

MintClientg q
)q r
;r s
} 	
public## 
IClient## 
CreateClient## #
(### $
string##$ *
name##+ /
,##/ 0
Action##1 7
<##7 8
CreateClientOptions##8 K
>##K L
?##L M
configureClient##N ]
=##^ _
null##` d
)##d e
=>$$ 
name$$ 
==$$ 
null$$ 
?$$ 
throw$$ #
new$$$ '!
ArgumentNullException$$( =
($$= >
nameof$$> D
($$D E
name$$E I
)$$I J
)$$J K
:$$L M
clients%% 
.%% 
GetOrAdd%% 
(%% 
name%% !
,%%! "
(%%# $
n%%$ %
)%%% &
=>%%' )
{&& 
var'' 
options'' 
='' 
new'' !
CreateClientOptions''" 5
(''5 6
createHttpClient''6 F
)''F G
;''G H
configureClient(( 
?((  
.((  !
Invoke((! '
(((' (
options((( /
)((/ 0
;((0 1
return)) 
createClientFunc)) '
())' (
n))( )
,))) *
options))+ 2
)))2 3
;))3 4
}** 
)** 
;** 
private.. 
IClient.. 

MintClient.. "
(.." #
string..# )
name..* .
,... /
CreateClientOptions..0 C
createClientOptions..D W
)..W X
{// 	
return55 
new55 
Client55 
(55 
createClientOptions66 #
.66# $ 
SerializationAdapter66$ 8
,668 9
createClientOptions77 #
.77# $
BaseUrl77$ +
,77+ ,
createClientOptions88 #
.88# $!
DefaultRequestHeaders88$ 9
,889 :
loggerFactory99 
?99 
.99 
CreateLogger99 +
<99+ ,
Client99, 2
>992 3
(993 4
)994 5
,995 6
createClientOptions:: #
.::# $
CreateHttpClient::$ 4
,::4 5
createClientOptions;; #
.;;# $"
SendHttpRequestMessage;;$ :
,;;: ;
createClientOptions<< #
.<<# $!
GetHttpRequestMessage<<$ 9
,<<9 :
createClientOptions== #
.==# $#
ThrowExceptionOnFailure==$ ;
,==; <
name>> 
)>> 
;>> 
}?? 	
}AA 
}BB £m
RC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\ClientExtensions.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

static 
class 
ClientExtensions (
{ 
public 
static 
Client 
With !
(! "
this" &
Client' -
client. 4
,4 5
AbsoluteUrl6 A
baseUriB I
)I J
=> 

client 
!= 
null 
? 
new  
Client! '
(' (
client 
.  
SerializationAdapter '
,' (
baseUri 
, 
client 
. !
DefaultRequestHeaders (
,( )
client 
. 
logger 
is 
ILogger $
<$ %
Client% +
>+ ,
logger- 3
?4 5
logger6 <
:= >
null? C
,C D
client 
. 
createHttpClient #
,# $
client 
. "
sendHttpRequestMessage )
,) *
client 
. !
getHttpRequestMessage (
,( )
client 
. #
ThrowExceptionOnFailure *
,* +
client 
. 
Name 
) 
: 
throw  
new! $!
ArgumentNullException% :
(: ;
nameof; A
(A B
clientB H
)H I
)I J
;J K
public$$ 
static$$ 
Client$$ 
With$$ !
($$! "
this$$" &
Client$$' -
client$$. 4
,$$4 5
IHeadersCollection$$6 H!
defaultRequestHeaders$$I ^
)$$^ _
=>%% 

client&& 
!=&& 
null&& 
?&& 
new&&  
Client&&! '
(&&' (
client'' 
.''  
SerializationAdapter'' '
,''' (
client(( 
.(( 
BaseUrl(( 
,(( !
defaultRequestHeaders)) !
,))! "
client** 
.** 
logger** 
is** 
ILogger** $
<**$ %
Client**% +
>**+ ,
logger**- 3
?**4 5
logger**6 <
:**= >
null**? C
,**C D
client++ 
.++ 
createHttpClient++ #
,++# $
client,, 
.,, "
sendHttpRequestMessage,, )
,,,) *
client-- 
.-- !
getHttpRequestMessage-- (
,--( )
client.. 
... #
ThrowExceptionOnFailure.. *
,..* +
client// 
.// 
Name// 
)// 
:// 
throw//  
new//! $!
ArgumentNullException//% :
(//: ;
nameof//; A
(//A B
client//B H
)//H I
)//I J
;//J K
public44 
static44 
Client44 %
WithDefaultRequestHeaders44 6
(446 7
this447 ;
Client44< B
client44C I
,44I J
string44K Q
key44R U
,44U V
string44W ]
value44^ c
)44c d
=>55 
With55 
(55 
client55 
,55 
key55 
.55  
ToHeadersCollection55  3
(553 4
value554 9
)559 :
)55: ;
;55; <
public:: 
static:: 
Client:: 
With:: !
(::! "
this::" &
Client::' -
client::. 4
,::4 5
ILogger::6 =
<::= >
Client::> D
>::D E
logger::F L
)::L M
=>;; 

client<< 
!=<< 
null<< 
?<< 
new<<  
Client<<! '
(<<' (
client== 
.==  
SerializationAdapter== '
,==' (
client>> 
.>> 
BaseUrl>> 
,>> 
client?? 
.?? !
DefaultRequestHeaders?? (
,??( )
logger@@ 
,@@ 
clientAA 
.AA 
createHttpClientAA #
,AA# $
clientBB 
.BB "
sendHttpRequestMessageBB )
,BB) *
clientCC 
.CC !
getHttpRequestMessageCC (
,CC( )
clientDD 
.DD #
ThrowExceptionOnFailureDD *
,DD* +
clientEE 
.EE 
NameEE 
)EE 
:EE 
throwEE  
newEE! $!
ArgumentNullExceptionEE% :
(EE: ;
nameofEE; A
(EEA B
clientEEB H
)EEH I
)EEI J
;EEJ K
publicJJ 
staticJJ 
ClientJJ 
WithJJ !
(JJ! "
thisJJ" &
ClientJJ' -
clientJJ. 4
,JJ4 5!
ISerializationAdapterJJ6 K 
serializationAdapterJJL `
)JJ` a
=>KK( *
clientLL 
!=LL 
nullLL 
?LL 
newLL  
ClientLL! '
(LL' ( 
serializationAdapterMM  
,MM  !
clientNN 
.NN 
BaseUrlNN 
,NN 
clientOO 
.OO !
DefaultRequestHeadersOO (
,OO( )
clientPP 
.PP 
loggerPP 
isPP 
ILoggerPP $
<PP$ %
ClientPP% +
>PP+ ,
loggerPP- 3
?PP4 5
loggerPP6 <
:PP= >
nullPP? C
,PPC D
clientQQ 
.QQ 
createHttpClientQQ #
,QQ# $
clientRR 
.RR "
sendHttpRequestMessageRR )
,RR) *
clientSS 
.SS !
getHttpRequestMessageSS (
,SS( )
clientTT 
.TT #
ThrowExceptionOnFailureTT *
,TT* +
clientUU 
.UU 
NameUU 
)UU 
:UU 
throwUU  
newUU! $!
ArgumentNullExceptionUU% :
(UU: ;
nameofUU; A
(UUA B
clientUUB H
)UUH I
)UUI J
;UUJ K
publicZZ 
staticZZ 
ClientZZ 
WithZZ !
(ZZ! "
thisZZ" &
ClientZZ' -
clientZZ. 4
,ZZ4 5
CreateHttpClientZZ6 F
createHttpClientZZG W
)ZZW X
=>[[ 
client\\ 
!=\\ 
null\\ 
?\\  
new\\! $
Client\\% +
(\\+ ,
client]] 
.]]  
SerializationAdapter]] /
,]]/ 0
client^^ 
.^^ 
BaseUrl^^ "
,^^" #
client__ 
.__ !
DefaultRequestHeaders__ 0
,__0 1
client`` 
.`` 
logger`` !
is``" $
ILogger``% ,
<``, -
Client``- 3
>``3 4
logger``5 ;
?``< =
logger``> D
:``E F
null``G K
,``K L
createHttpClientaa $
,aa$ %
clientbb 
.bb "
sendHttpRequestMessagebb 1
,bb1 2
clientcc 
.cc !
getHttpRequestMessagecc 0
,cc0 1
clientdd 
.dd #
ThrowExceptionOnFailuredd 2
,dd2 3
clientee 
.ee 
Nameee 
)ee  
:ee! "
throwee# (
newee) ,!
ArgumentNullExceptionee- B
(eeB C
nameofeeC I
(eeI J
clienteeJ P
)eeP Q
)eeQ R
;eeR S
publicjj 
staticjj 
Clientjj 
Withjj !
(jj! "
thisjj" &
Clientjj' -
clientjj. 4
,jj4 5"
IGetHttpRequestMessagejj6 L!
getHttpRequestMessagejjM b
)jjb c
=>kk 
clientll 
!=ll 
nullll 
?ll  
newll! $
Clientll% +
(ll+ ,
clientmm 
.mm  
SerializationAdaptermm /
,mm/ 0
clientnn 
.nn 
BaseUrlnn "
,nn" #
clientoo 
.oo !
DefaultRequestHeadersoo 0
,oo0 1
clientpp 
.pp 
loggerpp !
ispp" $
ILoggerpp% ,
<pp, -
Clientpp- 3
>pp3 4
loggerpp5 ;
?pp< =
loggerpp> D
:ppE F
nullppG K
,ppK L
clientqq 
.qq 
createHttpClientqq +
,qq+ ,
clientrr 
.rr "
sendHttpRequestMessagerr 1
,rr1 2!
getHttpRequestMessagess )
,ss) *
clienttt 
.tt #
ThrowExceptionOnFailurett 2
,tt2 3
clientuu 
.uu 
Nameuu 
)uu  
:uu! "
throwuu# (
newuu) ,!
ArgumentNullExceptionuu- B
(uuB C
nameofuuC I
(uuI J
clientuuJ P
)uuP Q
)uuQ R
;uuR S
publiczz 
staticzz 
Clientzz 
Withzz !
(zz! "
thiszz" &
Clientzz' -
clientzz. 4
,zz4 5#
ISendHttpRequestMessagezz6 M"
sendHttpRequestMessagezzN d
)zzd e
=>{{ 
client|| 
!=|| 
null|| 
?||  
new||! $
Client||% +
(||+ ,
client}} 
.}}  
SerializationAdapter}} /
,}}/ 0
client~~ 
.~~ 
BaseUrl~~ "
,~~" #
client 
. !
DefaultRequestHeaders 0
,0 1
client
ÄÄ 
.
ÄÄ 
logger
ÄÄ !
is
ÄÄ" $
ILogger
ÄÄ% ,
<
ÄÄ, -
Client
ÄÄ- 3
>
ÄÄ3 4
logger
ÄÄ5 ;
?
ÄÄ< =
logger
ÄÄ> D
:
ÄÄE F
null
ÄÄG K
,
ÄÄK L
client
ÅÅ 
.
ÅÅ 
createHttpClient
ÅÅ +
,
ÅÅ+ ,$
sendHttpRequestMessage
ÇÇ *
,
ÇÇ* +
client
ÉÉ 
.
ÉÉ #
getHttpRequestMessage
ÉÉ 0
,
ÉÉ0 1
client
ÑÑ 
.
ÑÑ %
ThrowExceptionOnFailure
ÑÑ 2
,
ÑÑ2 3
client
ÖÖ 
.
ÖÖ 
Name
ÖÖ 
)
ÖÖ  
:
ÖÖ! "
throw
ÖÖ# (
new
ÖÖ) ,#
ArgumentNullException
ÖÖ- B
(
ÖÖB C
nameof
ÖÖC I
(
ÖÖI J
client
ÖÖJ P
)
ÖÖP Q
)
ÖÖQ R
;
ÖÖR S
public
ãã 
static
ãã 
Client
ãã 
With
ãã !
(
ãã! "
this
ãã" &
Client
ãã' -
client
ãã. 4
,
ãã4 5
bool
ãã6 :%
throwExceptionOnFailure
ãã; R
)
ããR S
=>
åå 
client
çç 
!=
çç 
null
çç 
?
çç  
new
çç! $
Client
çç% +
(
çç+ ,
client
éé 
.
éé "
SerializationAdapter
éé /
,
éé/ 0
client
èè 
.
èè 
BaseUrl
èè "
,
èè" #
client
êê 
.
êê #
DefaultRequestHeaders
êê 0
,
êê0 1
client
ëë 
.
ëë 
logger
ëë !
is
ëë" $
ILogger
ëë% ,
<
ëë, -
Client
ëë- 3
>
ëë3 4
logger
ëë5 ;
?
ëë< =
logger
ëë> D
:
ëëE F
null
ëëG K
,
ëëK L
client
íí 
.
íí 
createHttpClient
íí +
,
íí+ ,
client
ìì 
.
ìì $
sendHttpRequestMessage
ìì 1
,
ìì1 2
client
îî 
.
îî #
getHttpRequestMessage
îî 0
,
îî0 1%
throwExceptionOnFailure
ïï +
,
ïï+ ,
client
ññ 
.
ññ 
Name
ññ 
)
ññ  
:
ññ! "
throw
ññ# (
new
ññ) ,#
ArgumentNullException
ññ- B
(
ññB C
nameof
ññC I
(
ññI J
client
ññJ P
)
ññP Q
)
ññQ R
;
ññR S
public
õõ 
static
õõ 
Client
õõ 
With
õõ !
(
õõ! "
this
õõ" &
Client
õõ' -
client
õõ. 4
,
õõ4 5
string
õõ6 <
name
õõ= A
)
õõA B
=>
úú 
client
ùù 
!=
ùù 
null
ùù 
?
ùù  
new
ùù! $
Client
ùù% +
(
ùù+ ,
client
ûû 
.
ûû "
SerializationAdapter
ûû /
,
ûû/ 0
client
üü 
.
üü 
BaseUrl
üü "
,
üü" #
client
†† 
.
†† #
DefaultRequestHeaders
†† 0
,
††0 1
client
°° 
.
°° 
logger
°° !
is
°°" $
ILogger
°°% ,
<
°°, -
Client
°°- 3
>
°°3 4
logger
°°5 ;
?
°°< =
logger
°°> D
:
°°E F
null
°°G K
,
°°K L
client
¢¢ 
.
¢¢ 
createHttpClient
¢¢ +
,
¢¢+ ,
client
££ 
.
££ $
sendHttpRequestMessage
££ 1
,
££1 2
client
§§ 
.
§§ #
getHttpRequestMessage
§§ 0
,
§§0 1
client
•• 
.
•• %
ThrowExceptionOnFailure
•• 2
,
••2 3
name
¶¶ 
)
¶¶ 
:
¶¶ 
throw
¶¶ !
new
¶¶" %#
ArgumentNullException
¶¶& ;
(
¶¶; <
nameof
¶¶< B
(
¶¶B C
client
¶¶C I
)
¶¶I J
)
¶¶J K
;
¶¶K L
}
ΩΩ 
}ææ éV
VC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\ClientCallExtensions.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public		 

static		 
class		  
ClientCallExtensions		 ,
{

 
public 
static 
async 
Task  
<  !
(! "
IClient" )
Client* 0
,0 1
Response2 :
<: ;
TResponseBody; H
>H I
ResponseJ R
)R S
>S T
GetAsyncU ]
<] ^
TResponseBody^ k
>k l
(l m
this 
AbsoluteUrl 
url  
,  !
IHeadersCollection 
? 
headersCollection  1
=2 3
null4 8
,8 9
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
if 
( 
url 
== 
null 
) 
throw "
new# &!
ArgumentNullException' <
(< =
nameof= C
(C D
urlD G
)G H
)H I
;I J
var 
client 
= 
new 
Client #
(# $
url$ '
.' (
WithRelativeUrl( 7
(7 8
RelativeUrl8 C
.C D
EmptyD I
)I J
)J K
;K L
var 
response 
= 
await  
client! '
.' (
GetAsync( 0
<0 1
TResponseBody1 >
>> ?
(? @
url 
. 
RelativeUrl 
,  
headersCollection !
,! "
cancellationToken !
)! "
. 
ConfigureAwait 
(  
false  %
)% &
??' )
throw* /
new0 3%
InvalidOperationException4 M
(M N
$strN e
)e f
;f g
return 
( 
client 
, 
response $
)$ %
;% &
} 	
public 
static 
async 
Task  
<  !
(! "
IClient" )
Client* 0
,0 1
Response2 :
<: ;
TResponseBody; H
>H I
ResponseJ R
)R S
>S T

PatchAsyncU _
<_ `
TResponseBody` m
,m n
TRequestBodyo {
>{ |
(| }
this 
AbsoluteUrl 
url  
,  !
TRequestBody 
? 
requestBody %
=& '
default( /
,/ 0
IHeadersCollection 
? 
headersCollection  1
=2 3
null4 8
,8 9
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
if   
(   
url   
==   
null   
)   
throw   "
new  # &!
ArgumentNullException  ' <
(  < =
nameof  = C
(  C D
url  D G
)  G H
)  H I
;  I J
var!! 
client!! 
=!! 
new!! 
Client!! #
(!!# $
url!!$ '
.!!' (
WithRelativeUrl!!( 7
(!!7 8
RelativeUrl!!8 C
.!!C D
Empty!!D I
)!!I J
)!!J K
;!!K L
var"" 
response"" 
="" 
await""  
client""! '
.""' (

PatchAsync""( 2
<""2 3
TResponseBody""3 @
,""@ A
TRequestBody""B N
>""N O
(""O P
requestBody## 
,## 
url$$ 
.$$ 
RelativeUrl$$ 
,$$  
headersCollection%% !
,%%! "
cancellationToken&& !
)&&! "
.'' 
ConfigureAwait'' 
(''  
false''  %
)''% &
??''' )
throw''* /
new''0 3%
InvalidOperationException''4 M
(''M N
$str''N e
)''e f
;''f g
return(( 
((( 
client(( 
,(( 
response(( $
)(($ %
;((% &
})) 	
public++ 
static++ 
async++ 
Task++  
<++  !
(++! "
IClient++" )
Client++* 0
,++0 1
Response++2 :
<++: ;
TResponseBody++; H
>++H I
Response++J R
)++R S
>++S T
	PostAsync++U ^
<++^ _
TResponseBody++_ l
,++l m
TRequestBody++n z
>++z {
(++{ |
this,, 
AbsoluteUrl,, 
url,,  
,,,  !
TRequestBody-- 
?-- 
requestBody-- %
=--& '
default--( /
,--/ 0
IHeadersCollection.. 
?.. 
headersCollection..  1
=..2 3
null..4 8
,..8 9
CancellationToken// 
cancellationToken// /
=//0 1
default//2 9
)//9 :
{00 	
if11 
(11 
url11 
==11 
null11 
)11 
throw11 "
new11# &!
ArgumentNullException11' <
(11< =
nameof11= C
(11C D
url11D G
)11G H
)11H I
;11I J
var22 
client22 
=22 
new22 
Client22 #
(22# $
url22$ '
.22' (
WithRelativeUrl22( 7
(227 8
RelativeUrl228 C
.22C D
Empty22D I
)22I J
)22J K
;22K L
var33 
response33 
=33 
await33  
client33! '
.33' (
	PostAsync33( 1
<331 2
TResponseBody332 ?
,33? @
TRequestBody33A M
>33M N
(33N O
requestBody44 
,44 
url55 
.55 
RelativeUrl55 
,55  
headersCollection66 !
,66! "
cancellationToken77 !
)77! "
.88 
ConfigureAwait88 
(88  
false88  %
)88% &
??88' )
throw88* /
new880 3%
InvalidOperationException884 M
(88M N
$str88N e
)88e f
;88f g
return99 
(99 
client99 
,99 
response99 $
)99$ %
;99% &
}:: 	
public<< 
static<< 
async<< 
Task<<  
<<<  !
(<<! "
IClient<<" )
Client<<* 0
,<<0 1
Response<<2 :
<<<: ;
TResponseBody<<; H
><<H I
Response<<J R
)<<R S
><<S T
PutAsync<<U ]
<<<] ^
TResponseBody<<^ k
,<<k l
TRequestBody<<m y
><<y z
(<<z {
this== 
AbsoluteUrl== 
url==  
,==  !
TRequestBody>> 
?>> 
requestBody>> %
=>>& '
default>>( /
,>>/ 0
IHeadersCollection?? 
??? 
headersCollection??  1
=??2 3
null??4 8
,??8 9
CancellationToken@@ 
cancellationToken@@ /
=@@0 1
default@@2 9
)@@9 :
{AA 	
ifBB 
(BB 
urlBB 
==BB 
nullBB 
)BB 
throwBB "
newBB# &!
ArgumentNullExceptionBB' <
(BB< =
nameofBB= C
(BBC D
urlBBD G
)BBG H
)BBH I
;BBI J
varCC 
clientCC 
=CC 
newCC 
ClientCC #
(CC# $
urlCC$ '
.CC' (
WithRelativeUrlCC( 7
(CC7 8
RelativeUrlCC8 C
.CCC D
EmptyCCD I
)CCI J
)CCJ K
;CCK L
varDD 
responseDD 
=DD 
awaitDD  
clientDD! '
.DD' (
PutAsyncDD( 0
<DD0 1
TResponseBodyDD1 >
,DD> ?
TRequestBodyDD@ L
>DDL M
(DDM N
requestBodyEE 
,EE 
urlFF 
.FF 
RelativeUrlFF 
,FF  
headersCollectionGG !
,GG! "
cancellationTokenHH !
)HH! "
.II 
ConfigureAwaitII 
(II  
falseII  %
)II% &
??II' )
throwII* /
newII0 3%
InvalidOperationExceptionII4 M
(IIM N
$strIIN e
)IIe f
;IIf g
returnJJ 
(JJ 
clientJJ 
,JJ 
responseJJ $
)JJ$ %
;JJ% &
}KK 	
publicMM 
staticMM 
asyncMM 
TaskMM  
<MM  !
(MM! "
IClientMM" )
ClientMM* 0
,MM0 1
ResponseMM2 :
ResponseMM; C
)MMC D
>MMD E
DeleteAsyncMMF Q
(MMQ R
thisNN 
AbsoluteUrlNN 
urlNN  
,NN  !
IHeadersCollectionOO 
?OO 
headersCollectionOO  1
=OO2 3
nullOO4 8
,OO8 9
CancellationTokenPP 
cancellationTokenPP /
=PP0 1
defaultPP2 9
)PP9 :
{QQ 	
ifRR 
(RR 
urlRR 
==RR 
nullRR 
)RR 
throwRR "
newRR# &!
ArgumentNullExceptionRR' <
(RR< =
nameofRR= C
(RRC D
urlRRD G
)RRG H
)RRH I
;RRI J
varSS 
clientSS 
=SS 
newSS 
ClientSS #
(SS# $
urlSS$ '
.SS' (
WithRelativeUrlSS( 7
(SS7 8
RelativeUrlSS8 C
.SSC D
EmptySSD I
)SSI J
)SSJ K
;SSK L
varTT 
responseTT 
=TT 
awaitTT  
clientTT! '
.TT' (
DeleteAsyncTT( 3
(TT3 4
urlUU 
.UU 
RelativeUrlUU 
,UU  
headersCollectionVV !
,VV! "
cancellationTokenWW !
)WW! "
.XX 
ConfigureAwaitXX 
(XX  
falseXX  %
)XX% &
??XX' )
throwXX* /
newXX0 3%
InvalidOperationExceptionXX4 M
(XXM N
$strXXN e
)XXe f
;XXf g
returnYY 
(YY 
clientYY 
,YY 
responseYY $
)YY$ %
;YY% &
}ZZ 	
}\\ 
}]] ºπ
HC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.NET\Client.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

sealed 
class 
Client 
:  
IClient! (
,( )
IDisposable* 5
{ 
internal 
readonly 
CreateHttpClient *
createHttpClient+ ;
;; <
internal 
readonly "
IGetHttpRequestMessage 0!
getHttpRequestMessage1 F
;F G
internal!! 
readonly!! #
ISendHttpRequestMessage!! 1"
sendHttpRequestMessage!!2 H
;!!H I
internal&& 
readonly&& 
ILogger&& !
logger&&" (
;&&( )
internal-- 
readonly-- 
Lazy-- 
<-- 

HttpClient-- )
>--) *
lazyHttpClient--+ 9
;--9 :
internal00 
bool00 
Disposed00 
{00  
get00! $
;00$ %
set00& )
;00) *
}00+ ,
public>> 
Client>> 
(>> 
string@@ 
baseUrl@@ 
,@@ !
ISerializationAdapterEE 
?EE  
serializationAdapterEE 3
=EE4 5
nullEE6 :
,EE: ;
IHeadersCollectionGG 
?GG !
defaultRequestHeadersGG 1
=GG2 3
nullGG4 8
,GG8 9
ILoggerHH 
<HH 
ClientHH 
>HH 
?HH 
loggerHH 
=HH  !
nullHH" &
,HH& '
CreateHttpClientII 
?II 
createHttpClientII *
=II+ ,
nullII- 1
,II1 2#
ISendHttpRequestMessageJJ 
?JJ  
sendHttpRequestJJ! 0
=JJ1 2
nullJJ3 7
,JJ7 8"
IGetHttpRequestMessageKK 
?KK !
getHttpRequestMessageKK  5
=KK6 7
nullKK8 <
,KK< =
boolLL #
throwExceptionOnFailureLL $
=LL% &
trueLL' +
,LL+ ,
stringMM 
?MM 
nameMM 
=MM 
nullMM 
)MM 
:MM 
thisMM #
(MM# $ 
serializationAdapterNN  
,NN  !
baseUrlOO 
.OO 
ToAbsoluteUrlOO !
(OO! "
)OO" #
,OO# $!
defaultRequestHeadersPP !
,PP! "
loggerQQ 
,QQ 
createHttpClientRR 
,RR 
sendHttpRequestSS 
,SS !
getHttpRequestMessageTT !
,TT! "#
throwExceptionOnFailureUU #
,UU# $
nameVV 
)VV 
{WW 	
}XX 	
publiccc 
Clientcc 
(cc 
AbsoluteUrldd 
?dd 
baseUrldd 
,dd !
ISerializationAdapterhh 
?hh  
serializationAdapterhh 3
=hh4 5
nullhh6 :
,hh: ;
IHeadersCollectionjj 
?jj !
defaultRequestHeadersjj 1
=jj2 3
nulljj4 8
,jj8 9
ILoggerkk 
<kk 
Clientkk 
>kk 
?kk 
loggerkk 
=kk  !
nullkk" &
,kk& '
CreateHttpClientll 
?ll 
createHttpClientll *
=ll+ ,
nullll- 1
,ll1 2#
ISendHttpRequestMessagemm 
?mm  
sendHttpRequestmm! 0
=mm1 2
nullmm3 7
,mm7 8"
IGetHttpRequestMessagenn 
?nn !
getHttpRequestMessagenn  5
=nn6 7
nullnn8 <
,nn< =
booloo #
throwExceptionOnFailureoo $
=oo% &
trueoo' +
,oo+ ,
stringpp 
?pp 
namepp 
=pp 
nullpp 
)pp 
:pp 
thispp #
(pp# $ 
serializationAdapterqq  
,qq  !
baseUrlrr 
,rr !
defaultRequestHeadersss !
,ss! "
loggertt 
,tt 
createHttpClientuu 
,uu 
sendHttpRequestvv 
,vv !
getHttpRequestMessageww !
,ww! "#
throwExceptionOnFailurexx #
,xx# $
nameyy 
)yy 
{zz 	
}{{ 	
public
ÜÜ 
Client
ÜÜ 
(
ÜÜ #
ISerializationAdapter
ää 
?
ää "
serializationAdapter
ää 3
=
ää4 5
null
ää6 :
,
ää: ;
AbsoluteUrl
åå 
?
åå 
baseUrl
åå 
=
åå 
null
åå #
,
åå# $ 
IHeadersCollection
çç 
?
çç #
defaultRequestHeaders
çç 1
=
çç2 3
null
çç4 8
,
çç8 9
ILogger
éé 
<
éé 
Client
éé 
>
éé 
?
éé 
logger
éé 
=
éé  !
null
éé" &
,
éé& '
CreateHttpClient
èè 
?
èè 
createHttpClient
èè *
=
èè+ ,
null
èè- 1
,
èè1 2%
ISendHttpRequestMessage
êê 
?
êê  
sendHttpRequest
êê! 0
=
êê1 2
null
êê3 7
,
êê7 8$
IGetHttpRequestMessage
ëë 
?
ëë #
getHttpRequestMessage
ëë  5
=
ëë6 7
null
ëë8 <
,
ëë< =
bool
íí %
throwExceptionOnFailure
íí $
=
íí% &
true
íí' +
,
íí+ ,
string
ìì 
?
ìì 
name
ìì 
=
ìì 
null
ìì 
)
ìì 
{
îî 	#
DefaultRequestHeaders
ïï !
=
ïï" ##
defaultRequestHeaders
ïï$ 9
??
ïï: <#
NullHeadersCollection
ïï= R
.
ïïR S
Instance
ïïS [
;
ïï[ \
if
öö 
(
öö "
serializationAdapter
öö $
==
öö% '
null
öö( ,
)
öö, -
{
õõ "
SerializationAdapter
ùù $
=
ùù% &&
JsonSerializationAdapter
ùù' ?
.
ùù? @
Instance
ùù@ H
;
ùùH I#
DefaultRequestHeaders
üü %
=
üü& '#
defaultRequestHeaders
†† )
==
††* ,
null
††- 1
?
††2 3#
DefaultRequestHeaders
††4 I
.
††I J'
WithJsonContentTypeHeader
††J c
(
††c d
)
††d e
:
††f g#
defaultRequestHeaders
°° )
.
°°) *
Contains
°°* 2
(
°°2 3
HeadersExtensions
°°3 D
.
°°D E#
ContentTypeHeaderName
°°E Z
)
°°Z [
?
°°\ ]#
defaultRequestHeaders
¢¢ )
:
¢¢* +#
defaultRequestHeaders
¢¢, A
.
¢¢A B'
WithJsonContentTypeHeader
¢¢B [
(
¢¢[ \
)
¢¢\ ]
;
¢¢] ^
}
££ 
else
§§ 
{
•• "
SerializationAdapter
¶¶ $
=
¶¶% &"
serializationAdapter
¶¶' ;
;
¶¶; <
}
ßß 
this
™™ 
.
™™ 
logger
™™ 
=
™™ 
(
™™ 
ILogger
™™ "
?
™™" #
)
™™# $
logger
™™$ *
??
™™+ -

NullLogger
™™. 8
.
™™8 9
Instance
™™9 A
;
™™A B
BaseUrl
¨¨ 
=
¨¨ 
baseUrl
¨¨ 
??
¨¨  
AbsoluteUrl
¨¨! ,
.
¨¨, -
Empty
¨¨- 2
;
¨¨2 3
Name
ÆÆ 
=
ÆÆ 
name
ÆÆ 
??
ÆÆ 
Guid
ÆÆ 
.
ÆÆ  
NewGuid
ÆÆ  '
(
ÆÆ' (
)
ÆÆ( )
.
ÆÆ) *
ToString
ÆÆ* 2
(
ÆÆ2 3
)
ÆÆ3 4
;
ÆÆ4 5
this
∞∞ 
.
∞∞ #
getHttpRequestMessage
∞∞ &
=
∞∞' (#
getHttpRequestMessage
∞∞) >
??
∞∞? A*
DefaultGetHttpRequestMessage
∞∞B ^
.
∞∞^ _
Instance
∞∞_ g
;
∞∞g h
this
≤≤ 
.
≤≤ 
createHttpClient
≤≤ !
=
≤≤" #
createHttpClient
≤≤$ 4
??
≤≤5 7
new
≤≤8 ;
CreateHttpClient
≤≤< L
(
≤≤L M
(
≤≤M N
n
≤≤N O
)
≤≤O P
=>
≤≤Q S
new
≤≤T W

HttpClient
≤≤X b
(
≤≤b c
)
≤≤c d
)
≤≤d e
;
≤≤e f
lazyHttpClient
¥¥ 
=
¥¥ 
new
¥¥  
Lazy
¥¥! %
<
¥¥% &

HttpClient
¥¥& 0
>
¥¥0 1
(
¥¥1 2
(
¥¥2 3
)
¥¥3 4
=>
¥¥5 7
this
¥¥8 <
.
¥¥< =
createHttpClient
¥¥= M
(
¥¥M N
Name
¥¥N R
)
¥¥R S
)
¥¥S T
;
¥¥T U$
sendHttpRequestMessage
∂∂ "
=
∂∂# $
sendHttpRequest
∂∂% 4
??
∂∂5 7+
DefaultSendHttpRequestMessage
∂∂8 U
.
∂∂U V
Instance
∂∂V ^
;
∂∂^ _%
ThrowExceptionOnFailure
∏∏ #
=
∏∏$ %%
throwExceptionOnFailure
∏∏& =
;
∏∏= >
}
ππ 	
public
¬¬ 
AbsoluteUrl
¬¬ 
BaseUrl
¬¬ "
{
¬¬# $
get
¬¬% (
;
¬¬( )
init
¬¬* .
;
¬¬. /
}
¬¬0 1
public
««  
IHeadersCollection
«« !#
DefaultRequestHeaders
««" 7
{
««8 9
get
««: =
;
««= >
init
««? C
;
««C D
}
««E F
public
ÃÃ 
string
ÃÃ 
Name
ÃÃ 
{
ÃÃ 
get
ÃÃ  
;
ÃÃ  !
init
ÃÃ" &
;
ÃÃ& '
}
ÃÃ( )
public
—— #
ISerializationAdapter
—— $"
SerializationAdapter
——% 9
{
——: ;
get
——< ?
;
——? @
init
——A E
;
——E F
}
——G H
public
÷÷ 
bool
÷÷ %
ThrowExceptionOnFailure
÷÷ +
{
÷÷, -
get
÷÷. 1
;
÷÷1 2
init
÷÷3 7
;
÷÷7 8
}
÷÷9 :
public
‹‹ 
void
‹‹ 
Dispose
‹‹ 
(
‹‹ 
)
‹‹ 
{
›› 	
if
ﬁﬁ 
(
ﬁﬁ 
Disposed
ﬁﬁ 
)
ﬁﬁ 
return
ﬁﬁ  
;
ﬁﬁ  !
Disposed
‡‡ 
=
‡‡ 
true
‡‡ 
;
‡‡ 
lazyHttpClient
‚‚ 
.
‚‚ 
Value
‚‚  
?
‚‚  !
.
‚‚! "
Dispose
‚‚" )
(
‚‚) *
)
‚‚* +
;
‚‚+ ,
}
„„ 	
public
ÂÂ 
async
ÂÂ 
Task
ÂÂ 
<
ÂÂ 
Response
ÂÂ "
<
ÂÂ" #
TResponseBody
ÂÂ# 0
>
ÂÂ0 1
>
ÂÂ1 2
	SendAsync
ÂÂ3 <
<
ÂÂ< =
TResponseBody
ÂÂ= J
,
ÂÂJ K
TRequestBody
ÂÂL X
>
ÂÂX Y
(
ÂÂY Z
IRequest
ÂÂZ b
<
ÂÂb c
TRequestBody
ÂÂc o
>
ÂÂo p
request
ÂÂq x
)
ÂÂx y
{
ÊÊ 	
if
ÁÁ 
(
ÁÁ 
request
ÁÁ 
==
ÁÁ 
null
ÁÁ 
)
ÁÁ  
throw
ÁÁ! &
new
ÁÁ' *#
ArgumentNullException
ÁÁ+ @
(
ÁÁ@ A
nameof
ÁÁA G
(
ÁÁG H
request
ÁÁH O
)
ÁÁO P
)
ÁÁP Q
;
ÁÁQ R!
HttpResponseMessage
ÈÈ !
httpResponseMessage
ÈÈ  3
;
ÈÈ3 4
try
ÎÎ 
{
ÏÏ 
var
ÌÌ 

httpClient
ÌÌ 
=
ÌÌ  
lazyHttpClient
ÌÌ! /
.
ÌÌ/ 0
Value
ÌÌ0 5
??
ÌÌ6 8
throw
ÌÌ9 >
new
ÌÌ? B'
InvalidOperationException
ÌÌC \
(
ÌÌ\ ]
$str
ÌÌ] }
)
ÌÌ} ~
;
ÌÌ~ 
if
ÒÒ 
(
ÒÒ 

httpClient
ÒÒ 
.
ÒÒ 
BaseAddress
ÒÒ *
!=
ÒÒ+ -
null
ÒÒ. 2
)
ÒÒ2 3
{
ÚÚ 
throw
ÛÛ 
new
ÛÛ '
InvalidOperationException
ÛÛ 7
(
ÛÛ7 8
$"
ÛÛ8 :
$str
ÛÛ: V
{
ÛÛV W
nameof
ÛÛW ]
(
ÛÛ] ^

HttpClient
ÛÛ^ h
)
ÛÛh i
}
ÛÛi j
$str
ÛÛj r
{
ÛÛr s
nameof
ÛÛs y
(
ÛÛy z

HttpClientÛÛz Ñ
.ÛÛÑ Ö
BaseAddressÛÛÖ ê
)ÛÛê ë
}ÛÛë í
$strÛÛí ò
{ÛÛò ô
nameofÛÛô ü
(ÛÛü †

HttpClientÛÛ† ™
)ÛÛ™ ´
}ÛÛ´ ¨
$strÛÛ¨ ø
{ÛÛø ¿
nameofÛÛ¿ ∆
(ÛÛ∆ «

HttpClientÛÛ« —
.ÛÛ— “
BaseAddressÛÛ“ ›
)ÛÛ› ﬁ
}ÛÛﬁ ﬂ
$strÛÛﬂ ö
{ÛÛö õ
nameofÛÛõ °
(ÛÛ° ¢

HttpClientÛÛ¢ ¨
)ÛÛ¨ ≠
}ÛÛ≠ Æ
$strÛÛÆ ¥
{ÛÛ¥ µ
nameofÛÛµ ª
(ÛÛª º

HttpClientÛÛº ∆
.ÛÛ∆ «
BaseAddressÛÛ« “
)ÛÛ“ ”
}ÛÛ” ‘
"ÛÛ‘ ’
)ÛÛ’ ÷
;ÛÛ÷ ◊
}
ÙÙ 
if
ˆˆ 
(
ˆˆ 

httpClient
ˆˆ 
.
ˆˆ #
DefaultRequestHeaders
ˆˆ 4
.
ˆˆ4 5
Any
ˆˆ5 8
(
ˆˆ8 9
)
ˆˆ9 :
)
ˆˆ: ;
{
˜˜ 
throw
¯¯ 
new
¯¯ '
InvalidOperationException
¯¯ 7
(
¯¯7 8
$"
¯¯8 :
$str
¯¯: V
{
¯¯V W
nameof
¯¯W ]
(
¯¯] ^

HttpClient
¯¯^ h
)
¯¯h i
}
¯¯i j
$str¯¯j Ö
{¯¯Ö Ü
nameof¯¯Ü å
(¯¯å ç

HttpClient¯¯ç ó
.¯¯ó ò%
DefaultRequestHeaders¯¯ò ≠
)¯¯≠ Æ
}¯¯Æ Ø
$str¯¯Ø µ
{¯¯µ ∂
nameof¯¯∂ º
(¯¯º Ω

HttpClient¯¯Ω «
)¯¯« »
}¯¯» …
$str¯¯… ⁄
{¯¯⁄ €
nameof¯¯€ ·
(¯¯· ‚

HttpClient¯¯‚ Ï
.¯¯Ï Ì%
DefaultRequestHeaders¯¯Ì Ç
)¯¯Ç É
}¯¯É Ñ
$str¯¯Ñ ø
{¯¯ø ¿
nameof¯¯¿ ∆
(¯¯∆ «

HttpClient¯¯« —
)¯¯— “
}¯¯“ ”
$str¯¯” Ÿ
{¯¯Ÿ ⁄
nameof¯¯⁄ ‡
(¯¯‡ ·

HttpClient¯¯· Î
.¯¯Î Ï%
DefaultRequestHeaders¯¯Ï Å
)¯¯Å Ç
}¯¯Ç É
"¯¯É Ñ
)¯¯Ñ Ö
;¯¯Ö Ü
}
˘˘ 
logger
˚˚ 
.
˚˚ 
LogTrace
˚˚ 
(
˚˚  
Messages
˚˚  (
.
˚˚( )
TraceBeginSend
˚˚) 7
,
˚˚7 8
request
˚˚9 @
,
˚˚@ A

TraceEvent
˚˚B L
.
˚˚L M
Request
˚˚M T
,
˚˚T U
lazyHttpClient
˚˚V d
.
˚˚d e
Value
˚˚e j
,
˚˚j k#
SerializationAdapter˚˚l Ä
,˚˚Ä Å
(˚˚Ç É
object˚˚É â
?˚˚â ä
)˚˚ä ã
request˚˚ã í
.˚˚í ì
BodyData˚˚ì õ
??˚˚ú û
$str˚˚ü °
)˚˚° ¢
;˚˚¢ £!
httpResponseMessage
ÄÄ #
=
ÄÄ$ %
await
ÄÄ& +$
sendHttpRequestMessage
ÄÄ, B
.
ÄÄB C$
SendHttpRequestMessage
ÄÄC Y
(
ÄÄY Z
lazyHttpClient
ÅÅ "
.
ÅÅ" #
Value
ÅÅ# (
,
ÅÅ( )#
getHttpRequestMessage
ÇÇ )
,
ÇÇ) *
request
ÉÉ 
,
ÉÉ 
logger
ÑÑ 
,
ÑÑ "
SerializationAdapter
ÖÖ (
)
ÜÜ 
.
ÜÜ 
ConfigureAwait
ÜÜ $
(
ÜÜ$ %
false
ÜÜ% *
)
ÜÜ* +
;
ÜÜ+ ,
}
áá 
catch
àà 
(
àà #
TaskCanceledException
àà (
tce
àà) ,
)
àà, -
{
ââ 
logger
ää 
.
ää 
LogError
ää 
(
ää  
tce
ää  #
,
ää# $
Messages
ää% -
.
ää- . 
ErrorTaskCancelled
ää. @
,
ää@ A
request
ääB I
)
ääI J
;
ääJ K
throw
ãã 
;
ãã 
}
åå 
catch
çç 
(
çç $
MissingHeaderException
çç )
mhe
çç* -
)
çç- .
{
éé 
logger
èè 
.
èè 
LogError
èè 
(
èè  
mhe
èè  #
,
èè# $
mhe
èè% (
.
èè( )
Message
èè) 0
,
èè0 1
request
èè2 9
)
èè9 :
;
èè: ;
throw
êê 
;
êê 
}
ëë 
catch
íí 
(
íí 
	Exception
íí 
ex
íí 
)
íí  
{
ìì 
var
îî 
	exception
îî 
=
îî 
new
îî  #
SendException
îî$ 1
(
îî1 2
Messages
îî2 :
.
îî: ; 
ErrorSendException
îî; M
,
îîM N
request
îîO V
,
îîV W
ex
îîX Z
)
îîZ [
;
îî[ \
logger
ïï 
.
ïï 
LogError
ïï 
(
ïï  
	exception
ïï  )
,
ïï) *
Messages
ïï+ 3
.
ïï3 4 
ErrorSendException
ïï4 F
,
ïïF G
request
ïïH O
)
ïïO P
;
ïïP Q
throw
ññ 
	exception
ññ 
;
ññ  
}
óó 
logger
ôô 
.
ôô 
LogTrace
ôô 
(
ôô 
$str
ôô C
,
ôôC D
request
ôôE L
)
ôôL M
;
ôôM N
return
õõ 
await
õõ "
ProcessResponseAsync
õõ -
<
õõ- .
TResponseBody
õõ. ;
>
õõ; <
(
õõ< =
request
õõ= D
,
õõD E!
httpResponseMessage
õõF Y
)
õõY Z
.
õõZ [
ConfigureAwait
õõ[ i
(
õõi j
false
õõj o
)
õõo p
;
õõp q
}
úú 	
private
¢¢ 
async
¢¢ 
Task
¢¢ 
<
¢¢ 
Response
¢¢ #
<
¢¢# $
TResponseBody
¢¢$ 1
>
¢¢1 2
>
¢¢2 3"
ProcessResponseAsync
¢¢4 H
<
¢¢H I
TResponseBody
¢¢I V
>
¢¢V W
(
¢¢W X
IRequest
££ 
request
££ 
,
££ !
HttpResponseMessage
§§ !
httpResponseMessage
§§  3
)
§§3 4
{
•• 	
if
ßß 
(
ßß !
httpResponseMessage
ßß #
==
ßß$ &
null
ßß' +
)
ßß+ ,
throw
ßß- 2
new
ßß3 6#
ArgumentNullException
ßß7 L
(
ßßL M
nameof
ßßM S
(
ßßS T!
httpResponseMessage
ßßT g
)
ßßg h
)
ßßh i
;
ßßi j
var
©© 
responseData
©© 
=
©© 
await
©© $!
httpResponseMessage
©©% 8
.
©©8 9
Content
©©9 @
.
©©@ A"
ReadAsByteArrayAsync
©©A U
(
©©U V
)
©©V W
.
©©W X
ConfigureAwait
©©X f
(
©©f g
false
©©g l
)
©©l m
;
©©m n
var
´´ +
httpResponseHeadersCollection
´´ -
=
´´. /!
httpResponseMessage
´´0 C
.
´´C D
Headers
´´D K
.
´´K L!
ToHeadersCollection
´´L _
(
´´_ `
)
´´` a
;
´´a b
var
¨¨ 
contentHeaders
¨¨ 
=
¨¨  !
httpResponseMessage
¨¨! 4
.
¨¨4 5
Content
¨¨5 <
?
¨¨< =
.
¨¨= >
Headers
¨¨> E
?
¨¨E F
.
¨¨F G!
ToHeadersCollection
¨¨G Z
(
¨¨Z [
)
¨¨[ \
;
¨¨\ ]
var
≠≠ 

allHeaders
≠≠ 
=
≠≠ !
httpResponseMessage
≠≠ 0
.
≠≠0 1
Headers
≠≠1 8
.
≠≠8 9!
ToHeadersCollection
≠≠9 L
(
≠≠L M
)
≠≠M N
.
≠≠N O
Append
≠≠O U
(
≠≠U V
contentHeaders
≠≠V d
)
≠≠d e
;
≠≠e f
TResponseBody
ØØ 
?
ØØ 
responseBody
ØØ '
=
ØØ( )
default
ØØ* 1
;
ØØ1 2
if
±± 
(
±± !
httpResponseMessage
±± #
.
±±# $!
IsSuccessStatusCode
±±$ 7
)
±±7 8
{
≤≤ 
try
¥¥ 
{
µµ 
responseBody
∂∂  
=
∂∂! ""
SerializationAdapter
∂∂# 7
.
∂∂7 8
Deserialize
∂∂8 C
<
∂∂C D
TResponseBody
∂∂D Q
>
∂∂Q R
(
∂∂R S
responseData
∂∂S _
,
∂∂_ `+
httpResponseHeadersCollection
∂∂a ~
)
∂∂~ 
;∂∂ Ä
}
∑∑ 
catch
∏∏ 
(
∏∏ 
	Exception
∏∏  
ex
∏∏! #
)
∏∏# $
{
ππ 
var
∫∫ &
deserializationException
∫∫ 0
=
∫∫1 2
new
∫∫3 6&
DeserializationException
∫∫7 O
(
∫∫O P
Messages
∫∫P X
.
∫∫X Y)
ErrorMessageDeserialization
∫∫Y t
,
∫∫t u
responseData∫∫v Ç
,∫∫Ç É
ex∫∫Ñ Ü
)∫∫Ü á
;∫∫á à
logger
ºº 
.
ºº 
LogError
ºº #
(
ºº# $&
deserializationException
ºº$ <
,
ºº< =
Messages
ºº> F
.
ººF G)
ErrorMessageDeserialization
ººG b
,
ººb c
responseData
ººd p
)
ººp q
;
ººq r
throw
ææ &
deserializationException
ææ 2
;
ææ2 3
}
øø 
}
¿¿ 
var
¬¬ )
httpResponseMessageResponse
¬¬ +
=
¬¬, -
new
¬¬. 1
Response
¬¬2 :
<
¬¬: ;
TResponseBody
¬¬; H
>
¬¬H I
(
√√ 

allHeaders
ƒƒ 
,
ƒƒ 
(
≈≈ 
int
≈≈ 
)
≈≈ !
httpResponseMessage
≈≈ (
.
≈≈( )

StatusCode
≈≈) 3
,
≈≈3 4
request
∆∆ 
.
∆∆ 
HttpRequestMethod
∆∆ )
,
∆∆) *
responseData
«« 
,
«« 
responseBody
»» 
,
»» 
request
…… 
.
…… 
Uri
…… 
)
   
;
   
if
ÃÃ 
(
ÃÃ 
!
ÃÃ )
httpResponseMessageResponse
ÃÃ ,
.
ÃÃ, -
	IsSuccess
ÃÃ- 6
)
ÃÃ6 7
{
ÕÕ 
return
ŒŒ 
!
ŒŒ %
ThrowExceptionOnFailure
ŒŒ /
?
œœ )
httpResponseMessageResponse
œœ 1
:
–– 
throw
–– 
new
–– !
HttpStatusException
––  3
(
––3 4
Messages
——  
.
——  !'
GetErrorMessageNonSuccess
——! :
(
——: ;)
httpResponseMessageResponse
——; V
.
——V W

StatusCode
——W a
,
——a b)
httpResponseMessageResponse
““ 3
.
““3 4

RequestUri
““4 >
)
““> ?
,
““? @)
httpResponseMessageResponse
”” 3
)
””3 4
;
””4 5
}
‘‘ 
logger
÷÷ 
.
÷÷ 
LogTrace
÷÷ 
(
÷÷ 
Messages
÷÷ $
.
÷÷$ %$
TraceResponseProcessed
÷÷% ;
,
÷÷; <)
httpResponseMessageResponse
÷÷= X
,
÷÷X Y

TraceEvent
÷÷Z d
.
÷÷d e
Response
÷÷e m
)
÷÷m n
;
÷÷n o
return
ÿÿ )
httpResponseMessageResponse
ÿÿ .
;
ÿÿ. /
}
ŸŸ 	
}
‹‹ 
}›› 