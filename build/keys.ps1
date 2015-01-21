# Note: these values may only change during major release

If ($Version.Contains('-')) {

	# Use the development keys
	$Keys = @{
		'portable-net45' = '97d9e3aa481481c8'
	}

} Else {

	# Use the final release keys
	$Keys = @{
		'portable-net45' = 'cfeb5dbada5e1c25'
	}

}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
