¯
UC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\JsonModel\Person.cs
	namespace 	
ApiExamples
 
. 
Model 
. 
	JsonModel %
{ 
public 

class 
Person 
{ 
public		 
Guid		 
	PersonKey		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
=		, -
Guid		. 2
.		2 3
NewGuid		3 :
(		: ;
)		; <
;		< =
public

 
string

 
	FirstName

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Address 
BillingAddress %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
=4 5
new6 9
(9 :
): ;
;; <
public 
DateTime 
? 
DateOfBirth $
{% &
get' *
;* +
set, /
;/ 0
}1 2
=3 4
new5 8
DateTime9 A
(A B
$numB F
,F G
$numH I
,I J
$numK L
)L M
;M N
public 
DateTime 
	DateAdded !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
new2 5
(5 6
$num6 :
,: ;
$num< =
,= >
$num? @
)@ A
;A B
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
=$ %
$num& (
;( )
public 
decimal 
AccountBalance %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
=4 5
(6 7
decimal7 >
)> ?
$num? G
;G H
public 
double 
Weight 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
$num- 3
;3 4
} 
} Ó
VC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\JsonModel\Address.cs
	namespace 	
ApiExamples
 
. 
Model 
. 
	JsonModel %
{ 
public 

class 
Address 
{		 
public

 
Guid

 

AddressKey
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
$ %
set

& )
;

) *
}

+ ,
=

- .
Guid

/ 3
.

3 4
NewGuid

4 ;
(

; <
)

< =
;

= >
public 
string 
Street 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
$str- 6
;6 7
public 
string 
StreeNumber !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
Suburb 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} Ë
YC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\AuthenticationResult.cs
	namespace 	
ApiExamples
 
. 
Model 
{ 
public 

class  
AuthenticationResult %
{ 
public 
string 
BearerToken !
{" #
get$ '
;' (
set) ,
;, -
}. /
} 
}		 ê
ZC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\AuthenticationRequest.cs
	namespace 	
ApiExamples
 
. 
Model 
{ 
public 

class !
AuthenticationRequest &
{ 
public		 
string		 
ClientId		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
public

 
string

 
ClientSecret

 "
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
} 
} ž
NC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\ApiResult.cs
	namespace 	
ApiExamples
 
. 
Model 
{ 
public 

class 
	ApiResult 
{		 
public

 
string

 
Data

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 
List 
< 
string 
> 
Messages $
{% &
get' *
;* +
}, -
=. /
new0 3
(3 4
)4 5
;5 6
public 
List 
< 
string 
> 
Errors "
{# $
get% (
;( )
}* +
=, -
new. 1
(1 2
)2 3
;3 4
} 
} ù
PC:\Users\PC\Documents\GitHub\RestClient.Net\src\ApiExamples.Model\ApiMessages.cs
	namespace 	
ApiExamples
 
. 
Model 
{ 
public 

static 
class 
ApiMessages #
{ 
public 
const 
string 0
$SecureControllerNotAuthorizedMessage @
=A B
$strC S
;S T
public 
const 
string '
ErrorControllerErrorMessage 7
=8 9
$str: O
;O P
public 
const 
string -
!HeadersControllerExceptionMessage =
=> ?
$str@ ]
;] ^
}		 
}

 