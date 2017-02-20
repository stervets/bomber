public class CellController : ControllerBehaviour {
	public Cell cell;

	public Movable movable = Movable.Yes;
	public Blowable blowable = Blowable.Yes;
	public bool isLadder = false;

	protected override void OnAwake(params object[] args) {
	}

	protected override void OnStart(params object[] args) {
	}

	protected void UpdateCell(){
		cell.movable = (int)movable;
		cell.blowable = (int)blowable;
		cell.isLadder = isLadder;
	}

	protected virtual void InitCellController(){
	}

	public void BindCell(Cell _cell){
		cell = _cell;
		UpdateCell ();
	    InitCellController ();
	}
}
