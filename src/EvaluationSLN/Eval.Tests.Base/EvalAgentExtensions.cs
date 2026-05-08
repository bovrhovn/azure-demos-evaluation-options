namespace Eval.Tests.Base;

public class EvalAgentExtensions
{
    public static BinaryData GetRunData(string agentName, string responseId, string evaluationId)
    {
        object dataSource = new
        {
            type = "azure_ai_responses",
            item_generation_params = new
            {
                type = "response_retrieval",
                data_mapping = new { response_id = "{{item.resp_id}}" },
                source = new
                {
                    type = "file_content",
                    content = new[]
                    {
                        new
                        {
                            item = new { resp_id =  responseId}
                        }
                    }
                }
            },
        };
        return BinaryData.FromObjectAsJson(
            new
            {
                eval_id = evaluationId,
                name = $"Evaluation Run for Agent {agentName}",
                data_source = dataSource
            }
        );
    }

    public static BinaryData GetEvaluationConfig(string modelDeploymentName)
    {
        object[] testingCriteria = [
            new {
                type = "azure_ai_evaluator",
                name = "violence_detection",
                evaluator_name = "builtin.violence",
                data_mapping = new { query = "{{item.query}}", response = "{{sample.output_text}}" }
            },
        ];
        object dataSourceConfig = new
        {
            type = "azure_ai_source",
            scenario = "responses"
        };
        return BinaryData.FromObjectAsJson(
            new
            {
                name = "Agent Response Evaluation",
                data_source_config = dataSourceConfig,
                testing_criteria = testingCriteria
            }
        );
    }
}