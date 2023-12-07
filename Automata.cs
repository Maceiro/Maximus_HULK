
public class Automata {
  
  public string status;
  public int cursor;
  public char Value{ get; set; }
  public string Chain{ get; set; }
  public bool Inicial{ get { return Chain.Length==0 ; } } 
  public bool Final{ get; set; }

  public string Status { 
   get{ return status; }
  set { 
    if( value!="ID" && value!="Number" && value!="Op" && value!="string" ) throw new Exception("La clase no es valida") ; 
    else status= value ;
      }
     }

  public bool IsID() {  return Status== "ID"; }
  public bool IsNumber() {  return Status== "Number"; }
  public bool IsOp() {  return Status== "Op"; }
  public bool IsString() {  return Status=="string"; }
  public bool IsError { get; set; }

  public Automata() { Chain= ""; }

  public void Begin( List<Token> tokens, int i ) {

  if( Value.IsLetter()) {
    Chain= "" + Value; 
    Status= "ID";
    cursor= i;
  }
  if( Value.IsDigit()) {
    Chain= "" + Value; 
    Status= "Number";
    cursor= i;
  }
  if( Value.IsOpComp() ) {
    Chain= "" + Value; 
    Status= "Op";
    cursor= i;
  } 
  if( Value=='"') {
    Chain= " "; 
    Status="string";
    cursor= i;
  }
  if( Value.IsOpAritm() )  tokens.Add( new Token( "Op", Value + "", i));
  if( Value.IsOpBool() ) tokens.Add( new Token( "Op", Value + "", i ) );
  if( Value.IsSintax() ) tokens.Add( new Token( Value + "", Value + "", i));

  }

  public void Intermedio() {
  
  if( IsID() ) {
  if( Value.IsLetter() || Value.IsDigit() ) Chain+= Value;
  else Final= true;
  }

  if( IsNumber() ) {
    if( Value.IsDigit() ) Chain+= Value;
    else
    if( Value.IsLetter() ) {
      Status= "ID";
      Chain+= Value;
    }
    else Final= true ; 
  }

  if( IsOp() ) {
    if( Value=='=') Chain+= Value;
     Final= true;
  }

  if( IsString() ) {
    if( Value!='"') Chain+= Value;
    else Final= true; 
  }

  }

 }

