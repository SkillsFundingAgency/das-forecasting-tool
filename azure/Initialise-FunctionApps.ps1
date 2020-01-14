param(
    [string[]]$FunctionAppNames,
    [string]$ResourceGroupName
)

$SubscriptionId = (Get-AzureRmContext).Subscription.Id
if(!$SubscriptionId){
    $SubscriptionId = (Get-AzureRmContext).Subscription.SubscriptionId
}
foreach ($FunctionApp in $FunctionAppNames) {
    Write-Host "Initialising $FunctionApp"
    $ResourceActionParams = @{
        Action     = "listsecrets"
        ResourceId = "/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroupName/providers/Microsoft.Web/sites/$FunctionApp/functions/InitialiseFunction"
        ApiVersion = "2016-08-01"
        Force      = $true
    }
    $TriggerUrl = (Invoke-AzureRmResourceAction @ResourceActionParams).trigger_url
    Invoke-RestMethod -Uri $TriggerUrl -UseBasicParsing
}
