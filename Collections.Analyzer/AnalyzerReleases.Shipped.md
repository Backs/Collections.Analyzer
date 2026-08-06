; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 0.3.0

### New Rules

 Rule ID | Category    | Severity | Notes                                 
---------|-------------|----------|---------------------------------------
 CI0011  | Performance | Warning  | NPlusOneQueryAnalyzer                 

## Release 0.2.15

### New Rules

 Rule ID | Category    | Severity | Notes                                 
---------|-------------|----------|---------------------------------------
 CI0010  | Performance | Warning  | DictionaryLookupDiagnostic            

## Release 0.2.14

### New Rules

 Rule ID | Category    | Severity | Notes                                 
---------|-------------|----------|---------------------------------------
 CI0001  | Performance | Warning  | StringToArrayDiagnostic               
 CI0002  | Performance | Warning  | ArrayToArrayDiagnostic                
 CI0003  | Performance | Warning  | RedundantEnumerableToArrayDiagnostic  
 CI0004  | Performance | Warning  | ListFromStringDiagnostic              
 CI0005  | Performance | Warning  | ToArrayLengthDiagnostic               
 CI0006  | Performance | Warning  | ListInitializerDiagnostic             
 CI0007  | Performance | Warning  | ConcurrentCollectionIsEmptyDiagnostic 
 CI0008  | Performance | Warning  | ArrayContainsToHashSetDiagnostic      
 CI0009  | Performance | Warning  | ListLoopDiagnostic                    