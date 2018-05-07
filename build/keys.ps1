# Note: these values may only change during major release

If ($Version.Contains('-')) {

	# Use the development keys
	$Keys = @{
	    'netstandard1.1' = '97d9e3aa481481c8'
		'net452' = '97d9e3aa481481c8'
	}

} Else {

	# Use the final release keys
	$Keys = @{
		'netstandard1.1' = 'cfeb5dbada5e1c25'
		'net452' = 'cfeb5dbada5e1c25'
	}

}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
