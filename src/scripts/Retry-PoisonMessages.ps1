[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$connectionString,

    [Parameter(Mandatory=$True)]
    [string]$inputQueueName
)


function Get-Queue{
    param([string]$queueName)
    write-verbose "Getting reference to queue: $queueName"
    $queue = Get-AzureStorageQueue -Context $storageContext -Name $queueName -ErrorAction Ignore
    if (-not $queue) {
        Write-Warning "Queue not found: $queueName" 
        break
    }
    Write-Host "Got queue reference to queue " $queueName
    return $queue
}

$poisonQueueName = $inputQueueName+"-poison"
$storageContext = New-AzureStorageContext -ConnectionString $connectionString

write-host "Moving messages from " $inputQueueName " to " $poisonQueueName

$inputQueue =Get-Queue -queueName $inputQueueName 
$poisonQueue = Get-Queue -queueName $poisonQueueName

Write-Verbose "Got queues, now retrying messages."

$invisibleTimeout = [System.TimeSpan]::FromSeconds(30)
$message = $poisonQueue.CloudQueue.GetMessage($invisibleTimeout)
if(-not $message) {
    Write-Warning "No visible messages found in queue: $inputQueueName"
    break
}

while ($message){
    $id = $message.Id
    write-verbose "Retrying message: $id"
    $destMessage = New-Object -TypeName Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage -ArgumentList $message.AsString
    $inputQueue.CloudQueue.AddMessage($destMessage)
    $poisonQueue.CloudQueue.DeleteMessage($message)
    $message = $poisonQueue.CloudQueue.GetMessage($invisibleTimeout)
}

write-host "Finished retrying messages"


