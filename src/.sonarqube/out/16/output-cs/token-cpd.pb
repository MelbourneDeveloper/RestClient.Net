Ê 
jC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.DependencyInjection\RestClientExtensions.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

static 
class  
RestClientExtensions ,
{		 
public 
static 
IServiceCollection (
AddRestClient) 6
(6 7
this 
IServiceCollection #
serviceCollection$ 5
,5 6
Action 
< 
CreateClientOptions &
>& '$
configureSingletonClient( @
)@ A
=> 
AddRestClient 
( 
serviceCollection .
,. /$
configureSingletonClient0 H
:H I$
configureSingletonClientJ b
,b c
createClientd p
:p q
nullr v
)v w
;w x
public 
static 
IServiceCollection (
AddRestClient) 6
(6 7
this 
IServiceCollection #
serviceCollection$ 5
,5 6
Action 
< 
CreateClientOptions &
>& '
?' ($
configureSingletonClient) A
=B C
nullD H
,H I
Func 
< 
string 
, 
CreateClientOptions ,
,, -
IServiceProvider. >
,> ?
IClient@ G
>G H
?H I
createClientJ V
=W X
nullY ]
)   
{!! 	
_"" 
="" 
serviceCollection"" !
.## 
AddSingleton## 
<## 
CreateHttpClient## *
>##* +
(##+ ,
(##, -
sp##- /
)##/ 0
=>##1 3
{$$ 
var%% -
!microsoftHttpClientFactoryWrapper%% 5
=%%6 7
new%%8 ;-
!MicrosoftHttpClientFactoryWrapper%%< ]
(%%] ^
sp%%^ `
.%%` a
GetRequiredService%%a s
<%%s t
IHttpClientFactory	%%t †
>
%%† ‡
(
%%‡ ˆ
)
%%ˆ ‰
)
%%‰ Š
;
%%Š ‹
return&& -
!microsoftHttpClientFactoryWrapper&& 8
.&&8 9
CreateClient&&9 E
;&&E F
}'' 
)'' 
.(( 
AddSingleton(( 
<(( 
CreateClient(( &
>((& '
(((' (
(((( )
sp(() +
)((+ ,
=>((- /
{)) 
var** 
clientFactory** !
=**" #
new**$ '
ClientFactory**( 5
(**5 6
sp++ 
.++ 
GetRequiredService++ )
<++) *
CreateHttpClient++* :
>++: ;
(++; <
)++< =
,++= >
sp,, 
.,, 

GetService,, !
<,,! "
ILoggerFactory,," 0
>,,0 1
(,,1 2
),,2 3
,,,3 4
createClient--  
!=--! #
null--$ (
?--) *
(--+ ,
name--, 0
,--0 1
options--2 9
)--9 :
=>--; =
createClient..  
...  !
Invoke..! '
(..' (
name..( ,
,.., -
options... 5
,..5 6
sp..7 9
)..9 :
:..; <
null..= A
)// 
;// 
return11 
clientFactory11 $
.11$ %
CreateClient11% 1
;111 2
}22 
)22 
;22 
_44 
=44 
serviceCollection44 !
.44! "
AddSingleton44" .
(44. /
(44/ 0
sp440 2
)442 3
=>444 6
sp55 
.55 
GetRequiredService55 !
<55! "
CreateClient55" .
>55. /
(55/ 0
)550 1
(551 2
$str552 >
,55> ?$
configureSingletonClient55@ X
)55X Y
)55Y Z
;55Z [
return77 
serviceCollection77 $
;77$ %
}88 	
}99 
}:: ö
wC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.DependencyInjection\MicrosoftHttpClientFactoryWrapper.cs
	namespace 	

RestClient
 
. 
Net 
{ 
public 

class -
!MicrosoftHttpClientFactoryWrapper 2
{ 
private 
snh 
. 
IHttpClientFactory &
HttpClientFactory' 8
{9 :
get; >
;> ?
}@ A
public -
!MicrosoftHttpClientFactoryWrapper 0
(0 1
snh1 4
.4 5
IHttpClientFactory5 G
httpClientFactoryH Y
)Y Z
=>[ ]
HttpClientFactory^ o
=p q
httpClientFactory	r ƒ
;
ƒ „
public 
snh 
. 

HttpClient 
CreateClient *
(* +
string+ 1
name2 6
)6 7
=>8 :
HttpClientFactory; L
.L M
CreateClientM Y
(Y Z
nameZ ^
)^ _
;_ `
} 
} ø
hC:\Users\PC\Documents\GitHub\RestClient.Net\src\RestClient.Net.DependencyInjection\GlobalSuppressions.cs
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
$str	~ ‰
,
‰ Š
Target
‹ ‘
=
’ “
$str
” §
)
§ ¨
]
¨ ©