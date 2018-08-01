[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$connectionString,

    [Parameter(Mandatory=$True)]
    [long]$accountId,

    [ValidateSet("PaymentPeriodEnd","LevyDeclaration")]
    [Parameter(Mandatory=$True)]
    [string]$projectionSource
)


function Get-Queue{
    param([string]$queueName)
    write-verbose "Getting reference to queue: $queueName"
    $queue = Get-AzureStorageQueue -Context $storageContext -Name $queueName -ErrorAction Ignore
    if (-not $queue) {
        Write-Warning "Queue not found: $queueName" 
        break
    }
    Write-Host "Got queue reference to queue: " $queueName
    return $queue
}

$invisibleTimeout = [System.TimeSpan]::FromSeconds(30)
$storageContext = New-AzureStorageContext -ConnectionString $connectionString
$generateProjectionsQueue = Get-Queue -queueName "forecasting-projections-generate-projections"
Write-Verbose "Got build projections queue, now generating the command."
$command = @{ "EmployerAccountId" = $accountId; "ProjectionSource" = $projectionSource  }
$message = New-Object -TypeName Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage -ArgumentList (ConvertTo-Json $command)
Write-Verbose "Now sending the message to trigger account projection."
$generateProjectionsQueue.CloudQueue.AddMessage($message)
Write-Host "Sent trigger projection message for account " $accountId ", projection source " $projectionSource